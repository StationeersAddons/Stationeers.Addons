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

            foreach (var addonScript in addonScripts)
            {
                scriptFiles.Add(Path.Combine(addonDirectory, addonScript));
            }

            Console.WriteLine("CD: " + Environment.CurrentDirectory);
            Console.WriteLine($"Compiling addon '{addonName}'...");

            // TODO: Disable trusted code
            Compiler.Compile(addonName, scriptFiles.ToArray(), true); // TODO: fix issue with missing files
        }
    }
}
