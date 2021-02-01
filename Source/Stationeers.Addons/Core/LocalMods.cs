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
        
        // Should be looking at the server 'default.ini' for mod locations serverside
        public static readonly string LocalModsDirectory = !LoaderManager.IsDedicatedServer ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Stationeers/mods/" : GetDedicatedServerModsDirectory();

        public static string[] GetLocalModDirectories(bool includeDebugPlugins = true, bool skipIfDebugPluginExists = false)
        {
            UnityEngine.Debug.Log(LocalModsDirectory);
            UnityEngine.Debug.Log(Directory.Exists(LocalModsDirectory));
            if ((LocalModsDirectory == null) || !Directory.Exists(LocalModsDirectory))
            {
                UnityEngine.Debug.Log("ModLoader ERROR: Could not locate mods directory, no mods getting initialized.");
                return new string[] { };
            }

            UnityEngine.Debug.Log($"Trying to load mods from {LocalModsDirectory}");

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

        private static string GetDedicatedServerModsDirectory()
        {
            if(File.Exists("default.ini"))
            {
                UnityEngine.Debug.Log("default.ini found!");
            }
            foreach(string line in File.ReadLines("default.ini"))
            {
                if (line.Contains("MODPATH="))
                {
                    UnityEngine.Debug.Log($"Found modpath: {line.Substring(8)}");
                    return line.Substring(8);
                }
            }
            return null;
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
