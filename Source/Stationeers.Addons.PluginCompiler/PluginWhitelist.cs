// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Stationeers.Addons.API;

namespace Stationeers.Addons.PluginCompiler
{
    public static class PluginWhitelist
    {
        private static readonly List<ISymbol> Whitelist = new List<ISymbol>();
        private static readonly List<ISymbol> Blacklist = new List<ISymbol>();
        private static readonly Dictionary<string, IAssemblySymbol> AssemblySymbols = new Dictionary<string, IAssemblySymbol>();

        public static void Initialize(Compilation compilation)
        {
            RegisterAssemblies(compilation);
            Whitelist.Clear();
            Blacklist.Clear();
            
            // TODO: Register symbols

            // System
            WhitelistTypes(
                typeof(object),
                typeof(string)
            );
            
            // Unity
            WhitelistTypes(
                typeof(UnityEngine.Debug)
            );

            WhitelistTypesNamespaces(
            );
            
            // API
            WhitelistTypes(
                typeof(IPlugin),
                typeof(Globals)
            );
            
            WhitelistTypesNamespaces(
                typeof(BundleManager)
            );
            
            // Game
            
            // Harmony
        }

        public static bool IsWhitelisted(ISymbol symbol)
        {
            switch (symbol)
            {
                // If this is just a namespace, pass it. Namespaces do not hurt us.
                case INamespaceSymbol _: return true;
                case INamedTypeSymbol namedTypeSymbol:
                    return IsWhitelisted(namedTypeSymbol);
                case IFieldSymbol _:
                    // Most likely only for blacklist...?
                    return false;
                case IMethodSymbol _:
                    // Most likely only for blacklist...?
                    return false;
                case IPropertySymbol _:
                    // Most likely only for blacklist...?
                    return false;
                case IEventSymbol _:
                    // Most likely only for blacklist...?
                    return false;
            }

            // Allow every other symbol type. TODO: Check if we're not missing something.
            // Tho, types should do it.
            return true;
        }

        private static bool IsWhitelisted(INamedTypeSymbol symbol)
        {
            // Check for blacklist
            if (Blacklist.Contains(symbol)) 
                return false;
            
            // Check for namespace blacklist
            if (Blacklist.Contains(symbol.ContainingNamespace)) 
                return false;
            
            // Allow the type when its whole namespace is whitelisted
            if (Whitelist.Contains(symbol.ContainingNamespace)) 
                return true;
            
            // Check for whitelist
            return Whitelist.Contains(symbol);
        }

        private static void WhitelistTypesNamespaces(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                Whitelist.Add(symbol.ContainingNamespace);
            }
        }

        private static void BlacklistTypesNamespaces(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                Blacklist.Add(symbol.ContainingNamespace);
            }
        }

        private static void WhitelistTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                Whitelist.Add( symbol);
            }
        }

        private static void BlacklistTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                Debug.Assert(type != null, "type is null!");
                var symbol = GetNamedTypeSymbol(type);
                Blacklist.Add(symbol);
            }
        }

        private static INamedTypeSymbol GetNamedTypeSymbol(Type type)
        {
            Debug.Assert(type.FullName != null, "type.FullName is null!");
            
            var assemblyName = type.Assembly.FullName;
            
            if (AssemblySymbols.TryGetValue(assemblyName, out var assemblySymbol))
            {
                var symbol = assemblySymbol.GetTypeByMetadataName(type.FullName);

                if (symbol == null)
                    throw new Exception($"Missing symbol '{assemblyName}' in assembly {assemblyName}!");

                return symbol;
            }

            throw new Exception($"Missing assembly symbol '{assemblyName}'!");
        }
        
        private static void RegisterAssemblies(Compilation compilation)
        {
            // TODO: Assembly cache (?)
            
            foreach (var reference in compilation.References)
            {
                var symbol = compilation.GetAssemblyOrModuleSymbol(reference);

                if (symbol is IAssemblySymbol assemblySymbol)
                {
                    var symbolName = assemblySymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    AssemblySymbols.Add(symbolName, assemblySymbol);
                }
            }
        }
    }
}