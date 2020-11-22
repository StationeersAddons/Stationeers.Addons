// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Stationeers.Addons.AddonManager
{
    public static class PluginCompiler
    {
        private static readonly string[] GameAssemblies = {
            "mscorlib.dll",
            "Assembly-CSharp.dll",
            "System.dll",
            "System.Core.dll",
            "UnityEngine.dll",
            "UnityEngine.CoreModule.dll",
        };

        private static readonly string[] AdditionalAssemblies = {
            "Stationeers.Addons.Loader.dll"
        };

        public static string Compile(string addonName, string[] sourceFiles, bool trustedCode = false)
        {
            // based on http://www.tugberkugurlu.com/archive/compiling-c-sharp-code-into-memory-and-executing-it-with-roslyn

            var syntaxTrees = new List<SyntaxTree>();

            // Parse all files
            foreach (var sourceFile in sourceFiles)
            {
                Console.WriteLine($"Compiling '{sourceFile}'...");

                if (!File.Exists(sourceFile))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Could not find source file '{sourceFile}'.");
                    Console.ResetColor();
                    return string.Empty;
                }

                var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile));

                if (!trustedCode)
                {
                    if (!ValidateSyntaxTree(syntaxTree, out var failMessage))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Source file '{sourceFile}' did not pass security check! Fail Message: '{failMessage}'.");
                        Console.ResetColor();
                        return string.Empty;
                    }
                }

                syntaxTrees.Add(syntaxTree);
            }

            // Setup reference list
            var references = new List<MetadataReference>();

            foreach (var file in GameAssemblies)
            {
                var reference = MetadataReference.CreateFromFile(Path.Combine(Environment.CurrentDirectory, "../rocketstation_Data/Managed/", file));
                references.Add(reference);
            }

            foreach (var file in AdditionalAssemblies)
            {
                var reference = MetadataReference.CreateFromFile(Path.Combine(Environment.CurrentDirectory, file));
                references.Add(reference);
            }

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            // Compile
            var assemblyName = addonName + "-Assembly";
            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees.ToArray(), references.ToArray(), options);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                var output = result.Diagnostics;

                if (!result.Success)
                {
                    foreach (var error in output)
                    {
                        switch (error.Severity)
                        {
                            case DiagnosticSeverity.Hidden: continue;
                            case DiagnosticSeverity.Info: continue;
                            case DiagnosticSeverity.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case DiagnosticSeverity.Error:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        Console.WriteLine(error.GetMessage());
                    }
                    Console.ResetColor();
                    return string.Empty;
                }

                // Output as AddonsCache/AddonName-Assembly.dll
                var assemblyFile = "../AddonsCache/" + assemblyName + ".dll";
                using (var fs = new FileStream(assemblyFile, FileMode.Create))
                {
                    fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
                }
                
                return assemblyFile;
            }
        }

        private static bool ValidateSyntaxTree(SyntaxTree syntaxTree, out string failMessage)
        {
            // TODO: validate check for blacklisted types etc.

            failMessage = string.Empty;
            return true;
        }
    }
}
