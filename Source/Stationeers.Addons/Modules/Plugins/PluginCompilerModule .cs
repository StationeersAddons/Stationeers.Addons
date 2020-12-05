// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts;
using Steamworks;
using UnityEngine;

namespace Stationeers.Addons.Modules.Plugins
{
    internal class PluginCompilerModule : IModule
    {
        internal readonly struct AddonPlugin
        {
            public readonly string AddonName;
            public readonly string AssemblyFile;

            public AddonPlugin(string addonName, string assemblyFile)
            {
                AddonName = addonName;
                AssemblyFile = assemblyFile;
            }
        }

        internal static readonly List<AddonPlugin> CompiledPlugins = new List<AddonPlugin>();

        public string LoadingCaption => "Compiling plugins...";

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            CompiledPlugins.Clear();

            Debug.Log("Starting to compile the plugins...");

            var pluginCompiler = new PluginCompiler();

            if (!Directory.Exists("AddonManager/AddonsCache"))
                Directory.CreateDirectory("AddonManager/AddonsCache");

            foreach (var workshopItemID in WorkshopManager.Instance.SubscribedItems)
            {
                // TODO: Read XML file and get the real addon name to show

                if (SteamUGC.GetItemInstallInfo(workshopItemID, out _, out var pchFolder, 1024U, out _))
                {
                    // TODO: Detect when plugin doesn't need to be compiled (not changed etc.)
                    // TODO: Add non-compiled (skipped) plugins to the CompiledPlugins list

                    var addonDirectory = pchFolder;

                    var addonName = "workshop-" + workshopItemID.m_PublishedFileId;
                    var assemblyName = addonName + "-Assembly"; // TODO: Make some shared project for string constants etc.
                    var assemblyFile = "AddonManager/AddonsCache/" + assemblyName + ".dll";

                    if (!Directory.Exists(addonDirectory))
                    {
                        Debug.LogWarning($"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!");
                        continue;
                    }

                    var addonScripts = Directory.GetFiles(pchFolder, "*.cs", SearchOption.AllDirectories);

                    if (addonScripts.Length == 0) continue;

                    if (File.Exists(assemblyFile))
                    {
                        Debug.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }

                    pluginCompiler.CompileScripts(addonName, addonDirectory, addonScripts);

                    CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                }
                yield return new WaitForEndOfFrame();
            }

            // Dispose the compiler
            pluginCompiler.Dispose();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
        }
    }
}