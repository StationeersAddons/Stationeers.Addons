// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Stationeers.Addons.PluginCompiler.Whitelists;

namespace Stationeers.Addons.PluginCompiler
{
    public class PluginWhitelist : IDisposable
    {
        public static PluginWhitelist Instance { get; private set; }
        
        private readonly List<ISymbol> _whitelist = new List<ISymbol>();
        private readonly List<ISymbol> _blacklist = new List<ISymbol>();
        private readonly List<MemberInfo> _memberWhitelist = new List<MemberInfo>();
        private readonly List<MemberInfo> _memberBlacklist = new List<MemberInfo>();
        private readonly Dictionary<string, IAssemblySymbol> _assemblySymbols = new Dictionary<string, IAssemblySymbol>();

        private static readonly IWhitelistRegistry[] Whitelists =
        {
            new SystemWhitelist(),
            new UnityWhitelist(),
            new AddonsWhitelist(),
            new GameWhitelist(),
            new HarmonyWhitelist(),
            new JetbrainsWhitelist(),
        };
        
        public void Initialize(Compilation compilation)
        {
            Instance = this;
            
            RegisterAssemblies(compilation);

            // Register all whitelists
            foreach (var registry in Whitelists)
                registry.Register(this);
        }

        public void Dispose()
        {
            _whitelist.Clear();
            _blacklist.Clear();
            Instance = null;
        }

        public bool IsAllowed(ISymbol symbol)
        {
            switch (symbol)
            {
                // If this is just a namespace, pass it. Namespaces do not hurt us.
                case INamespaceSymbol _: return true;
                
                // Check types
                case INamedTypeSymbol namedTypeSymbol:
                    return IsWhitelisted(namedTypeSymbol);

                case IEventSymbol _:
                    // For events, we just check the namespace. It should be enough.
                    return IsWhitelisted(symbol.ContainingNamespace);

                // Check members
                case IFieldSymbol _:
                case IMethodSymbol _:
                case IPropertySymbol _:
                    // Now, nor the namespace or type is whitelisted.
                    // Check for members (it can be method, field and property)
                    return IsMemberWhitelisted(symbol);
            }

            // Allow every other symbol type. TODO: Check if we're not missing something.
            // Tho, types should do it.
            return true;
        }

        private bool IsWhitelisted(ISymbol symbol)
        {
            // Check if type is not in blacklist and it is in our whitelist
            return !_blacklist.Contains(symbol) && _whitelist.Contains(symbol);
        }

        private bool IsMemberWhitelisted(ISymbol symbol) // TODO: Refactor so we can use blacklist
        {
            // We allow all fields to be used.
            // This should be fine, as fields do not call any methods underneath,
            // and should not expose any "unsafe" data.
            if (symbol is IFieldSymbol)
                return true;
            
            // If the type symbol is whitelisted, allow.
            if (IsWhitelisted(symbol))
                return true;
            
            // If the whole namespace is whitelisted, allow.
            if (IsWhitelisted(symbol.ContainingNamespace))
                return true;

            switch (symbol)
            {
                case IMethodSymbol methodSymbol:
                    return IsMethodWhitelisted(methodSymbol);
                
                case IPropertySymbol propertySymbol:
                    return IsPropertyWhitelisted(propertySymbol);
                
                default:
                    return false;
            }
        }

        private bool IsMethodWhitelisted(IMethodSymbol symbol)
        {
            // This has to be recursive, as we have to check for overrides etc.
            
            if (symbol.IsOverride)
            {
                // TODO: Handle this
                return false;
            }

            foreach (var member in _memberWhitelist)
            {
                // At first, check if the symbol is contained within the member's declaring type
                var memberDeclType = GetNamedTypeSymbol(member.DeclaringType);
                if (memberDeclType != symbol.ContainingType) continue;

                // Check all registered members for methods, compare param length, names etc.
                //var members = memberDeclType.GetMembers().Where(x => x is MethodInfo);
            }

            return false;
        }

        private bool IsPropertyWhitelisted(IPropertySymbol symbol)
        {
            foreach (var member in _memberWhitelist)
            {
                // At first, check if the symbol is contained within the member's declaring type
                var memberDeclType = GetNamedTypeSymbol(member.DeclaringType);
                if (memberDeclType != symbol.ContainingType) continue;
                
                // Now, if any member in the member's declaring type.
                // If so, this is all we need to check, if property is in the list.
                if (memberDeclType.GetMembers().Any(x => x.Name == symbol.MetadataName))
                    return true;
            }
            
            return false;
        }

        public void WhitelistTypesNamespaces(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                _whitelist.Add(symbol.ContainingNamespace);
            }
        }

        public void WhitelistTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                _whitelist.Add(symbol);
            }
        }

        public void BlacklistTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                _blacklist.Add(symbol);
            }
        }

        public void WhitelistMembers(params MemberInfo[] members)
        {
            // TODO: Do we actually need to store the MemberInfo or just the INamedTypeSymbol of the declaring type...?
            
            foreach (var member in members)
            {
                Debug.Assert(member != null, "member is null!");
                _memberWhitelist.Add(member);
            }
        }

        public void BlacklistMembers(params MemberInfo[] members)
        {
            foreach (var member in members)
            {
                Debug.Assert(member != null, "member is null!");
                _memberBlacklist.Add(member);
            }                                                                                          
        }
        
        private INamedTypeSymbol GetNamedTypeSymbol(Type type)
        {
            Debug.Assert(type.FullName != null, "type.FullName is null!");
            
            var assemblyName = type.Assembly.FullName;
            
            if (_assemblySymbols.TryGetValue(assemblyName, out var assemblySymbol))
            {
                var symbol = assemblySymbol.GetTypeByMetadataName(type.FullName);

                if (symbol == null)
                    throw new Exception($"Missing symbol '{assemblyName}' in assembly {assemblyName}!");

                return symbol;
            }

            throw new Exception($"Missing assembly symbol '{assemblyName}'!");
        }
        
        private void RegisterAssemblies(Compilation compilation)
        {
            // TODO: Assembly cache (?)
            
            foreach (var reference in compilation.References)
            {
                var symbol = compilation.GetAssemblyOrModuleSymbol(reference);

                if (symbol is IAssemblySymbol assemblySymbol)
                {
                    var symbolName = assemblySymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    _assemblySymbols.Add(symbolName, assemblySymbol);
                }
            }
        }
    }
}