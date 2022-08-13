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
    internal class PluginWhitelist : IDisposable
    {
        public static PluginWhitelist Instance { get; private set; }
        
        private readonly List<string> _namespaceWhitelist = new List<string>();
        private readonly List<string> _typeWhitelist = new List<string>();
        private readonly List<string> _typeBlacklist = new List<string>();
        private readonly List<MemberInfo> _memberWhitelist = new List<MemberInfo>();
        private readonly List<string> _memberTypeWhitelist = new List<string>();
        
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
            _namespaceWhitelist.Clear();
            _typeWhitelist.Clear();
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
                    return IsTypeWhitelisted(namedTypeSymbol);
                
                // Check events
                case IEventSymbol _:
                    // For events, we just check the namespace. It should be enough.
                    return IsNamespaceWhitelisted(symbol.ContainingNamespace);

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

        private bool IsNamespaceWhitelisted(INamespaceSymbol symbol)
        {
            return _namespaceWhitelist.Contains(symbol.ToDisplayString());
        }

        private bool IsTypeWhitelisted(INamedTypeSymbol symbol, bool skipMemberTypeCheck = false)
        {
            // Note: I do not like the skipMemberTypeCheck parameter, TODO: Figure out a way, how to clean this up
            
            var symbolName = symbol.ToDisplayString();
            
            // Check the blacklist at first
            if (_typeBlacklist.Contains(symbolName)) return false;
            
            // If type's namespace is whitelisted, then always allow it.
            if (IsNamespaceWhitelisted(symbol.ContainingNamespace))
                return true;
            
            // Check if the type is whitelisted, and not blacklisted
            if (_typeWhitelist.Contains(symbolName))
                return true;

            // This is a last check, allow this type, only when member from this type is whitelisted.
            if (skipMemberTypeCheck) return false;
            
            return _memberTypeWhitelist.Contains(symbolName);
        }

        private bool IsMemberWhitelisted(ISymbol symbol)
        {
            // We allow all fields to be used.
            // This should be fine, as fields do not call any methods underneath,
            // and should not expose any "unsafe" data.
            if (symbol is IFieldSymbol)
                return true;

            // If the type symbol is whitelisted, allow.
            if (IsTypeWhitelisted(symbol.ContainingType, true))
                return true;
            
            // If the whole namespace is whitelisted, allow.
            if (IsNamespaceWhitelisted(symbol.ContainingNamespace))
                return true;
            
            // Handle delegates
            if (symbol is INamedTypeSymbol delegateSymbol && delegateSymbol.SpecialType == SpecialType.System_Delegate) // TODO: Does not work for event delegates, fix
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
                // symbol = symbol.GetOverriddenSymbol(); etc.
                return false;
            }
            
            // This part is the same for properties TODO: Refactor
            foreach (var member in _memberWhitelist)
            {
                // At first, check if the symbol is contained within the member's declaring type
                var memberDeclType = GetNamedTypeSymbol(member.DeclaringType);
                if (!SymbolEqualityComparer.Default.Equals(memberDeclType, symbol.ContainingType)) continue;

                // Now, if any member in the member's declaring type.
                // If so, this is all we need to check, if property is in the list.
                if (memberDeclType.GetMembers().Any(x => x.MetadataName == symbol.MetadataName) && member.Name == symbol.Name)
                    return true;
            }

            return false;
        }

        private bool IsPropertyWhitelisted(IPropertySymbol symbol)
        { 
            foreach (var member in _memberWhitelist)
            {
                // At first, check if the symbol is contained within the member's declaring type
                var memberDeclType = GetNamedTypeSymbol(member.DeclaringType);
                if (!SymbolEqualityComparer.Default.Equals(memberDeclType, symbol.ContainingType)) continue;
                
                // Now, if any member in the member's declaring type.
                // If so, this is all we need to check, if property is in the list.
                if (memberDeclType.GetMembers().Any(x => x.MetadataName == symbol.MetadataName) && member.Name == symbol.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public void WhitelistTypesNamespaces(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                _namespaceWhitelist.Add(symbol.ContainingNamespace.ToDisplayString());
            }
        }

        public void WhitelistTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                _typeWhitelist.Add(symbol.ToDisplayString());
            }
        }

        public void BlacklistTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                _typeBlacklist.Add(symbol.ToDisplayString());
            }
        }

        public void WhitelistMembers(params MemberInfo[] members)
        {
            // TODO: Do we actually need to store the MemberInfo or just the INamedTypeSymbol of the declaring type...?
            
            foreach (var member in members)
            {
                Debug.Assert(member != null, "member is null!");
                Debug.Assert(member.DeclaringType != null, "member's declaring type is null!");
                
                // TODO: Find a way to check members for non-static types.
                // For now, we just allow only static classes, as we do not have a way, to check, if type
                // is being used only for member access (call, i.e.: System.IO.Path.Combine) or something like:
                // System.IO.Path myPath; (this is not a valid C# code, because Path is static, but you get the idea),
                // and not checking for static types would allow all member-declaring types to be used like so.
                //var isStaticDeclType = member.DeclaringType.IsAbstract && member.DeclaringType.IsSealed;
               // Debug.Assert(isStaticDeclType, "Cannot register members from non-static types! (yet)");

                // Register member
                _memberWhitelist.Add(member);
                
                // Register declaring type as well
                var symbol = GetNamedTypeSymbol(member.DeclaringType);
                _memberTypeWhitelist.Add(symbol.ToDisplayString());
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