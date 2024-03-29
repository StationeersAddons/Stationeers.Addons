﻿// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Transports;
using Stationeers.Addons.Core;
using Stationeers.Addons.PluginCompiler;
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

            AddonsLogger.Log("Starting plugins compilation...");

            if (!Directory.Exists("AddonManager/AddonsCache"))
                Directory.CreateDirectory("AddonManager/AddonsCache");

            // TODO: Cleanup duplicated code
            
            // Load local plugins (but ignore if there is Debug version of it)
            yield return LoadLocalPlugins();
            
            // Load workshop plugins (if client)
            if(!LoaderManager.IsDedicatedServer)
            {
                yield return LoadWorkshopPlugins();
            }
        }

        private IEnumerator LoadLocalPlugins()
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
                        AddonsLogger.Warning(
                            $"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!");
                        continue;
                    }

                    var addonScripts = Directory.GetFiles(addonDirectory, "*.cs", SearchOption.AllDirectories);

                    // Prevent mods that include the following files from defining duplicate attributes and screwing up compile.
                    // */Properties/*
                    // */bin/*
                    // */obj/*
                    var sourceFilesList = new List<string>(addonScripts);

                    sourceFilesList.RemoveAll(x => x.Contains(Path.DirectorySeparatorChar + "Properties" + Path.DirectorySeparatorChar));
                    sourceFilesList.RemoveAll(x => x.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar));
                    sourceFilesList.RemoveAll(x => x.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar));

                    addonScripts = sourceFilesList.ToArray();

                    if (addonScripts.Length == 0)
                    {
                        AddonsLogger.Warning($"No scripts found in addon '{addonName}'!");
                        continue;
                    }

                    if (File.Exists(assemblyFile))
                    {
                        AddonsLogger.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }

                    // Compile addon source code
                    if (!Compiler.Compile(addonName, addonScripts))
                    {
                        AddonsLogger.Error($"Could not compile addon plugin '{addonName}'!");
                        continue;
                    }

                    CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                }
                catch (Exception ex)
                {
                    AddonsLogger.Error($"Failed to compile plugin from '{localPluginDirectory}'. Exception:\n{ex}");
                }
                
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LoadWorkshopPlugins()
        {
            yield return null;

            var query = NetworkManager.GetLocalAndWorkshopItems(SteamTransport.WorkshopType.Mod).GetAwaiter();
            // Somehow GetLocalAndWorkshopItems should return local mods as well, but it doesn't... ? TODO: When plugins are loading twice, check this call above and fix it.

            while (!query.IsCompleted) // This is not how UniTask should be used, but it works for now. 
                yield return null;

            var result = query.GetResult();

            foreach (var itemWrap in result)
            {
                // Ignore local mods
                if (itemWrap.IsLocal()) continue;

                try
                {
                    var addonDirectory = itemWrap.DirectoryPath;
                
                    var addonName = "workshop-" + itemWrap.Id;
                    var assemblyName = addonName + "-Assembly"; // TODO: Make some shared project for string constants etc.
                    var assemblyFile = "AddonManager/AddonsCache/" + assemblyName + ".dll";
                
                    if (!Directory.Exists(addonDirectory))
                    {
                        AddonsLogger.Warning($"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!");
                        continue;
                    }
                
                    var addonScripts = Directory.GetFiles(addonDirectory, "*.cs", SearchOption.AllDirectories);
                
                    if (addonScripts.Length == 0) continue;
                
                    if (File.Exists(assemblyFile))
                    {
                        AddonsLogger.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }
                    
                    // Check if the addon has been updated before we added the sandbox, if so, make it trusted
                    var isTrusted = itemWrap.LastWriteTime < Constants.SandboxIntroductionDate;

                    if (isTrusted)
                    {
                        AddonsLogger.Warning($"Addon '{addonName}' is trusted! (backwards-compatibility)");
                    }
                    
                    // Compile addon source code
                    if (!Compiler.Compile(addonName, addonScripts, isTrusted))
                    {
                        AddonsLogger.Error($"Could not compile addon plugin '{addonName}'!");
                        continue;
                    }

                    CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                }
                catch (Exception ex)
                {
                    AddonsLogger.Error($"Failed to compile plugin from '{itemWrap.DirectoryPath}'. Exception:\n{ex}");
                }
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
        }
    }
}