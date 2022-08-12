// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Stationeers.Addons.Core;
using UnityEngine;

// TODO: Handle instruction counter exceptions

namespace Stationeers.Addons.PluginCompiler.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PluginAnalyzer : DiagnosticAnalyzer
    {
#pragma warning disable RS2008
        private static readonly DiagnosticDescriptor DiagnosticError = new DiagnosticDescriptor(
            "ProhibitedCode",
            "Prohibited code",
            "{0} is prohibited!",
            "Whitelist",
            DiagnosticSeverity.Error,
            true
        );
#pragma warning restore RS2008

        private static readonly SyntaxKind[] SyntaxKinds =
        {
            SyntaxKind.FinallyClause,
            SyntaxKind.AliasQualifiedName,
            SyntaxKind.QualifiedName,
            SyntaxKind.GenericName,
            SyntaxKind.IdentifierName
        };

        public PluginAnalyzer()
        {
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticError);
        }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                                   GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKinds);
        }

        private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var info = context.SemanticModel.GetSymbolInfo(node);

            // How symbol can be null...?
            if (info.Symbol == null)
                return;
            
            // When this symbol is created locally (by the plugin's author), then always allow it.
            if (IsLocalSymbol(info.Symbol))
                return;

            // When this symbol is whitelisted, we're allowing it.
            if (PluginWhitelist.Instance.IsAllowed(info.Symbol))
                return;
            
            // We have failed all the diagnostics' tests. So it means that this symbol is prohibited.
            var symbolName = info.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            var report = Diagnostic.Create(DiagnosticError, node.GetLocation(), symbolName);
            context.ReportDiagnostic(report);

#if DEBUG
            AddonsLogger.Error($"Missing symbol ({symbolName}) namespace: {info.Symbol.ContainingNamespace}");
#endif
        }

        private static bool IsLocalSymbol(ISymbol symbol)
        {
            return symbol.Locations.All(symbolLocation => symbolLocation.IsInSource);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    }
}