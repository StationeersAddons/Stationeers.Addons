// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Transports;
using Stationeers.Addons.Core;
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

                    // Prevent mods that include the following files from defining duplicate attributes and screwing up compile.
                    // */Properties/*
                    // */bin/*
                    // */obj/*
                    List<string> sourceFilesList = new List<string>(addonScripts);

                    sourceFilesList.RemoveAll(x => x.Contains(Path.DirectorySeparatorChar + "Properties" + Path.DirectorySeparatorChar));
                    sourceFilesList.RemoveAll(x => x.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar));
                    sourceFilesList.RemoveAll(x => x.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar));

                    addonScripts = sourceFilesList.ToArray();

                    if (addonScripts.Length == 0) continue;

                    if (File.Exists(assemblyFile))
                    {
                        Debug.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }

                    pluginCompiler.CompileScripts(addonName, addonDirectory, addonScripts, out var isSuccess);

                    if (isSuccess)
                    {
                        CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                    }
                    else
                    {
                        throw new Exception(
                            $"Addon's plugin ('{addonName}') failed to compile. Checks game logs for more info.");
                    }
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
            yield return null;

            var query = NetManager.GetLocalAndWorkshopItems(SteamTransport.WorkshopType.Mod).GetAwaiter();
            // Somehow GetLocalAndWorkshopItems should return local mods as well, but it doesn't... ? TODO: When plugins are loading twice, check this call above and fix it.

            while (!query.IsCompleted) // This is not how UniTask should be used, but it works for now. 
                yield return null;

            var result = query.GetResult();

            foreach (var itemWrap in result)
            {
                try
                {
                    var addonDirectory = itemWrap.DirectoryPath;
                
                    var addonName = "workshop-" + itemWrap.Id;
                    var assemblyName = addonName + "-Assembly"; // TODO: Make some shared project for string constants etc.
                    var assemblyFile = "AddonManager/AddonsCache/" + assemblyName + ".dll";
                
                    if (!Directory.Exists(addonDirectory))
                    {
                        Debug.LogWarning($"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!");
                        continue;
                    }
                
                    var addonScripts = Directory.GetFiles(addonDirectory, "*.cs", SearchOption.AllDirectories);
                
                    if (addonScripts.Length == 0) continue;
                
                    if (File.Exists(assemblyFile))
                    {
                        Debug.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }
                
                    pluginCompiler.CompileScripts(addonName, addonDirectory, addonScripts, out var isSuccess);
                
                    if (isSuccess)
                    {
                        CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                    }
                    else
                    {
                        throw new Exception(
                            $"Addon's plugin ('{addonName}') failed to compile. Checks game logs for more info.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to compile plugin from '{itemWrap.DirectoryPath}'. Exception:\n{ex}");
                }
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
        }
    }
}