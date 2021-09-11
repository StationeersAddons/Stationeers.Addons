// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts;
using Stationeers.Addons.Core;
using Steamworks;
using UnityEngine;

// TODO: Read XML file and get the real addon name to show
// TODO: Detect when plugin doesn't need to be compiled (not changed etc.)
// TODO: Add non-compiled (skipped) plugins to the CompiledPlugins list

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

            Debug.Log("Starting plugins compilation...");

            var pluginCompiler = new PluginCompiler();

            if (!Directory.Exists("AddonManager/AddonsCache"))
                Directory.CreateDirectory("AddonManager/AddonsCache");

            // TODO: Cleanup duplicated code
            
            // Load local plugins (but ignore if there is Debug version of it)
            yield return LoadLocalPlugins(pluginCompiler);
            
            // Load workshop plugins (if client)
            if(!LoaderManager.IsDedicatedServer)
            {
                yield return LoadWorkshopPlugins(pluginCompiler);
            }
            
            // Dispose the compiler
            pluginCompiler.Dispose();
        }

        private IEnumerator LoadLocalPlugins(PluginCompiler pluginCompiler)
        {
            var localPluginDirectories = LocalMods.GetLocalModDirectories(false, true);
            
            foreach (var localPluginDirectory in localPluginDirectories)
            {
                try
                {
                    var addonDirectory = localPluginDirectory;

                    var addonName = "local-" + new DirectoryInfo(localPluginDirectory).Name;
                    var assemblyName = addonName + "-Assembly"; // TODO: Make some shared project for string constants etc.
                    var assemblyFile = "AddonManager/AddonsCache/" + assemblyName + ".dll";

                    if (!Directory.Exists(addonDirectory))
                    {
                        Debug.LogWarning(
                            $"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!");
                        continue;
                    }

                    var addonScripts = Directory.GetFiles(addonDirectory, "*.cs", SearchOption.AllDirectories);

                    if (addonScripts.Length == 0) continue;

                    if (File.Exists(assemblyFile))
                    {
                        Debug.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }

                    pluginCompiler.CompileScripts(addonName, addonDirectory, addonScripts);

                    CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to compile plugin from '{localPluginDirectory}'. Exception:\n{ex}");
                }
                
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LoadWorkshopPlugins(PluginCompiler pluginCompiler)
        {
            while(WorkshopManager.Instance == null) // Wait until WorkshopManager has started
            {
                yield return null;
            }

            foreach (var workshopItemID in WorkshopManager.Instance.SubscribedItems)
            {
                if (SteamUGC.GetItemInstallInfo(workshopItemID, out _, out var pchFolder, 1024U, out _))
                {
                    try
                    {
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
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to compile plugin from '{pchFolder}'. Exception:\n{ex}");
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
        }
    }
}