// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;

namespace Stationeers.Addons.PluginCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var addonName = Environment.GetEnvironmentVariable("AddonName");
            var addonDirectory = Environment.GetEnvironmentVariable("AddonDirectory");
            var addonScripts = Environment.GetEnvironmentVariable("AddonScripts").Split(';');

            var scriptFiles = new List<string>();

            Console.WriteLine($"Addon directory '{addonDirectory}'");

            foreach (var addonScript in addonScripts)
            {
                var addonScriptFile = addonDirectory + addonScript;
                scriptFiles.Add(addonScriptFile);
            }

            Console.WriteLine($"Compiling addon '{addonName}'...");

            // TODO: Disable trusted code
            var output = Compiler.Compile(addonName, scriptFiles.ToArray(), true);

            if (string.IsNullOrEmpty(output))
            {
                Console.WriteLine($"ERROR: Failed to compile addon '{addonName}'!");
            }
        }
    }
}
