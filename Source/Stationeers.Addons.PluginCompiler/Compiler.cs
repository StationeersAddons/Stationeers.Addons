// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
            "UnityEngine.UNETModule.dll",
            "UnityEngine.ParticleSystemModule.dll",
            "UnityEngine.PhysicsModule.dll",
            "UnityEngine.StreamingModule.dll",
            "UnityEngine.SubstanceModule.dll",
            "UnityEngine.UmbraModule.dll",
            "UnityEngine.TextCoreModule.dll",
            "UnityEngine.TextRenderingModule.dll",
            "UnityEngine.SharedInternalsModule.dll",
            "UnityEngine.IMGUIModule.dll",
            "UnityEngine.InputLegacyModule.dll",
            "UnityEngine.VideoModule.dll",
            "UnityEngine.JSONSerializeModule.dll",

            "com.unity.multiplayer-hlapi.Runtime.dll",

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

            // Check server/game instance (might make this more thorough or include it as part of signature)
            string installDirectory = Directory.Exists("../rocketstation_Data/Managed/") ? "../rocketstation_Data/Managed/" : "../rocketstation_DedicatedServer_Data/Managed/";

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
            var compilation = CSharpCompilation.Create(assemblyName)
                .WithReferences(references.ToArray())
                .WithOptions(options)
                .AddSyntaxTrees(syntaxTrees);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                var output = result.Diagnostics;

                if (!result.Success)
                {
                    foreach (var error in output)
                    {
                        var prefix = "";
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

        private static bool ValidateSyntaxTree(SyntaxTree syntaxTree, out string failMessage)
        {
            // TODO: validate check for blacklisted types etc.

            failMessage = string.Empty;
            return true;
        }
    }
}
