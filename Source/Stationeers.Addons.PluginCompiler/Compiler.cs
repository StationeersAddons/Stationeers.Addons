// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Stationeers.Addons.PluginCompiler.Analyzers;

namespace Stationeers.Addons.PluginCompiler
{
    public static class Compiler
    {
        private static readonly string[] GameAssemblies = {
            "mscorlib.dll",
            "netstandard.dll",
            "System.dll",
            "System.Core.dll",
            "System.Data.dll",
            "System.Xml.dll",

            "Assembly-CSharp.dll",
            "Assembly-CSharp-firstpass.dll",

            "UnityEngine.dll",
            "UnityEngine.CoreModule.dll",
            "UnityEngine.AssetBundleModule.dll",
            "UnityEngine.UI.dll",
            "UnityEngine.UIModule.dll",
            "UnityEngine.ParticleSystemModule.dll",
            "UnityEngine.PhysicsModule.dll",
            "UnityEngine.StreamingModule.dll",
            "UnityEngine.SubstanceModule.dll",
            "UnityEngine.UmbraModule.dll",
            "UnityEngine.TextCoreFontEngineModule.dll",
            "UnityEngine.TextCoreTextEngineModule.dll",
            "UnityEngine.TextRenderingModule.dll",
            "UnityEngine.SharedInternalsModule.dll",
            "UnityEngine.IMGUIModule.dll",
            "UnityEngine.InputLegacyModule.dll",
            "UnityEngine.VideoModule.dll",
            "UnityEngine.JSONSerializeModule.dll",

            "Unity.TextMeshPro.dll",
        };

        private static readonly string[] AdditionalAssemblies = {
            "Stationeers.Addons.dll",
            "0Harmony.dll"
        };

        public static string Compile(string addonName, string[] sourceFiles, bool trustedCode = false)
        {
            var syntaxTrees = new List<SyntaxTree>();

            // Parse all files
            foreach (var sourceFile in sourceFiles)
            {
                Console.WriteLine($"Compiling file '{sourceFile}'...");

                if (!File.Exists(sourceFile))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Could not find source file '{sourceFile}'.");
                    Console.ResetColor();
                    return string.Empty;
                }

                syntaxTrees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile)));
            }

            // Check server/game instance (might make this more thorough or include it as part of signature)
            var installDirectory = Directory.Exists("../rocketstation_Data/Managed/") ? "../rocketstation_Data/Managed/" : "../rocketstation_DedicatedServer_Data/Managed/";

            // Setup reference list
            var references = new List<MetadataReference>();

            foreach (var file in GameAssemblies)
            {
                var reference = MetadataReference.CreateFromFile(Path.Combine(Environment.CurrentDirectory, installDirectory, file));
                references.Add(reference);
            }

            foreach (var file in AdditionalAssemblies)
            {
                var reference = MetadataReference.CreateFromFile(Path.Combine(Environment.CurrentDirectory, file));
                references.Add(reference);
            }

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            // Compile
            Console.WriteLine($"Linking addon '{addonName}'...");
            
            var assemblyName = addonName + "-Assembly";
            var compilation = CSharpCompilation.Create($"{assemblyName}-{DateTime.UtcNow.Ticks}")
                .AddSyntaxTrees(syntaxTrees)
                .WithReferences(references.ToArray())
                .WithOptions(options);

            CompilationWithAnalyzers compilationWithAnalyzers = null;
            
            // Create compilation with additional analyzers for non-trusted code
            if (!trustedCode)
                compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new PluginAnalyzer()));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                var output = result.Diagnostics;

                // Run diagnostics only on non-trusted code
                if (!trustedCode)
                {
                    // Run diagnostics
                    var task = compilationWithAnalyzers.GetAllDiagnosticsAsync();
                    task.Wait();
                    var diagnostics = task.Result;
                    
                    // Fail, if we have any errors
                    var isSuccess = diagnostics.All(d => d.Severity != DiagnosticSeverity.Error);
                    
                    // Check diagnostics result for errors
                    CheckDiagnostics(diagnostics);

                    // If diagnostics result contains errors, we cannot compile this plugin
                    if (!isSuccess)
                    {
                        Console.ResetColor();
                        return string.Empty;
                    }
                }
                
                if (!result.Success)
                {
                    foreach (var error in output)
                    {
                        string prefix;
                        switch (error.Severity)
                        {
                            case DiagnosticSeverity.Hidden: 
                            case DiagnosticSeverity.Info: continue;
                            case DiagnosticSeverity.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                prefix = "[Plugin Compiler - WARNING]";
                                break;
                            case DiagnosticSeverity.Error:
                                Console.ForegroundColor = ConsoleColor.Red;
                                prefix = "[Plugin Compiler - ERROR]";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var errorMessage = error.GetMessage();

                        // Ignore mscorlib warnings for now.
                        // Task: https://trello.com/c/akuaJbdp/25-mscorlib-plugin-build-issues
                        if (errorMessage.Contains("mscorlib, Version=2.0.0.0,")) continue;

                        Console.WriteLine(prefix + " " + errorMessage);
                    }
                    Console.ResetColor();
                    return string.Empty;
                }

                // Output as AddonsCache/AddonName-Assembly.dll
                var assemblyFile = "AddonsCache/" + assemblyName + ".dll";
                using (var fs = new FileStream(assemblyFile, FileMode.Create))
                {
                    fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
                }
                
                return assemblyFile;
            }
        }

        private static void CheckDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            // Print-out all diagnostics
            foreach (var diagnostic in diagnostics)
            {
                var message = diagnostic.GetMessage();
                
                // Ignore warnings about mscorlib
                if (message.Contains("mscorlib, Version=2.0.0.0,")) continue;
                
                var location = diagnostic.Location.GetMappedLineSpan();
                var file = string.IsNullOrEmpty(location.Path) ? "unknown.cs" : location.Path; // TODO: Fix missing file names
                var line = location.StartLinePosition.Line;
                var character = location.StartLinePosition.Character;
                var severity = diagnostic.Severity.ToString().ToUpper();
                Console.WriteLine($"[Plugin Compiler - {severity}] {file}({line}, {character}): {message}");
            }
        }
    }
}
