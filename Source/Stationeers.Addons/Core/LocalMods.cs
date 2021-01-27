// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// TODO: We probably should refactor all of that.

namespace Stationeers.Addons.Core
{
    internal static class LocalMods
    {
        public const string DebugPluginPostfix = "-Debug";
        
        public static readonly string LocalModsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Stationeers/mods/";

        public static string[] GetLocalModDirectories(bool includeDebugPlugins = true, bool skipIfDebugPluginExists = false)
        {
            if (!Directory.Exists(LocalModsDirectory)) return new string[] { };

            var directories = Directory.GetDirectories(LocalModsDirectory);
            var modDirectory = new List<string>();

            foreach (var directory in directories)
            {
                if (!includeDebugPlugins && directory.EndsWith(DebugPluginPostfix)) continue;
                
                // Skip if this is not debug plugin and there is debug version of it
                // Messy, but works for now
                if (!directory.EndsWith(DebugPluginPostfix) && skipIfDebugPluginExists &&
                    directories.Any(x => x != directory && x.Contains(directory))) continue;
                
                modDirectory.Add(directory);
            }

            return modDirectory.ToArray();
        }

        public static string[] GetLocalModDebugAssemblies()
        {
            if (!Directory.Exists(LocalModsDirectory)) return new string[] { };
            
            var directories = GetLocalModDirectories();
            var modAssemblies = new List<string>();

            foreach (var directory in directories)
            {
                if (!directory.EndsWith(DebugPluginPostfix)) continue;
                var pluginName = Directory.GetParent(directory + "\\").Name + ".dll";
                var pluginDebugAssembly = Path.Combine(directory, pluginName);

                if (File.Exists(pluginDebugAssembly))
                    modAssemblies.Add(pluginDebugAssembly);
            }

            return modAssemblies.ToArray();
        }
    }
}
