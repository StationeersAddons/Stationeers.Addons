// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Stationeers.Addons.API;
using Stationeers.Addons.PluginCompiler.Whitelists;
using UnityEngine.Networking;

namespace Stationeers.Addons.PluginCompiler
{
    public class PluginWhitelist : IDisposable
    {
        public static PluginWhitelist Instance { get; private set; }
        
        private readonly List<ISymbol> _whitelist = new List<ISymbol>();
        private readonly List<ISymbol> _blacklist = new List<ISymbol>();
        private readonly Dictionary<string, IAssemblySymbol> _assemblySymbols = new Dictionary<string, IAssemblySymbol>();

        private static readonly IWhitelistRegistry[] Whitelists =
        {
            new SystemWhitelist(),
            new UnityWhitelist(),
            new AddonsWhitelist(),
            new GameWhitelist(),
            new HarmonyWhitelist(),
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

        public bool IsWhitelisted(ISymbol symbol)
        {
            switch (symbol)
            {
                // If this is just a namespace, pass it. Namespaces do not hurt us.
                case INamespaceSymbol _: return true;
                case INamedTypeSymbol namedTypeSymbol:
                    return IsWhitelisted(namedTypeSymbol);
                
                case IFieldSymbol _:
                case IMethodSymbol _:
                case IPropertySymbol _:
                case IEventSymbol _:
                    // Most likely only for blacklist...?
                    return false;
            }

            // Allow every other symbol type. TODO: Check if we're not missing something.
            // Tho, types should do it.
            return true;
        }

        private bool IsWhitelisted(INamedTypeSymbol symbol)
        {
            // Check for blacklist
            if (_blacklist.Contains(symbol)) 
                return false;
            
            // Allow the type when its whole namespace is whitelisted
            if (_whitelist.Contains(symbol.ContainingNamespace)) 
                return true;
            
            // Check for whitelist
            return _whitelist.Contains(symbol);
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
                _whitelist.Add( symbol);
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