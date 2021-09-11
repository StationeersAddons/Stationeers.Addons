// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Stationeers.Addons.PluginCompiler.Analyzers
{
    public static class PluginWhitelist
    {
        // TODO: ModuleTypeWhitelist array
        
        public static bool IsWhitelisted(ISymbol symbol)
        {
            switch (symbol)
            {
                // If this is just a namespace, pass it. Namespaces do not hurt us.
                case INamespaceSymbol _: return true;
                case INamedTypeSymbol _:
                    // TODO: Check types
                    // TODO: Watch out for overriden types!

                    var interfaces = symbol.GetType().GetInterfaces().Select(x => x.Name);
                    Console.WriteLine("Symbol type: " + symbol + $" ({string.Join(",", interfaces)})");
                    return false;
                case IFieldSymbol _:
                    // Most likely only for blacklist...?
                    break;
                case IMethodSymbol _:
                    // Most likely only for blacklist...?
                    break;
                case IPropertySymbol _:
                    // Most likely only for blacklist...?
                    break;
                case IEventSymbol _:
                    // Most likely only for blacklist...?
                    break;
            }

            // Allow every other symbol type. TODO: Check if we're not missing something.
            // Tho, types should do it.
            return true;
        }
    }
}