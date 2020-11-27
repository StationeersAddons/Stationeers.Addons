// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.IO;

namespace Stationeers.Addons.PluginCompiler
{
    internal class Program
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
                //var addonScriptFile  = Path.Combine(addonDirectory + addonScript); // TODO: Fix this shit, not working for some reason, because of an leading slash?

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
