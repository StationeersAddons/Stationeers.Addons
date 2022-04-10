// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Assets.Scripts.Networking;
using Stationeers.Addons.Core;
using UnityEngine;

namespace Stationeers.Addons.Modules.Plugins
{
    internal class PluginLoaderModule : IModule
    {
        internal struct PluginInfo
        {
            public Assembly Assembly { get; set; }
            public IPlugin[] Plugins { get; set; }
        }

        private readonly Dictionary<string, PluginInfo> _plugins = new Dictionary<string, PluginInfo>();

        public Dictionary<string, PluginInfo>.ValueCollection LoadedPlugins => _plugins.Values;
        public int NumLoadedPlugins => _plugins.Count;
        public string LoadingCaption => "Starting up plugins...";

        /// <inheritdoc />
        public void Initialize()
        {
            if (Directory.Exists("AddonManager/AddonsCacheTemp/"))
                Directory.Delete("AddonManager/AddonsCacheTemp/", true);
            
            Directory.CreateDirectory("AddonManager/AddonsCacheTemp/");
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            // Using PluginCompilerModule.CompiledPlugins we have to be sure that it has been created
            // TODO: Better way to reference cross-module or don't reference it at all.

            Debug.Log("Loading plugin assemblies...");

            foreach (var compiledPlugin in PluginCompilerModule.CompiledPlugins)
            {
                // TODO: Prevent from loading local addons

                Debug.Log($"Loading plugin assembly '{compiledPlugin.AssemblyFile}'");
                LoadPlugin(compiledPlugin.AddonName, compiledPlugin.AssemblyFile);
                yield return new WaitForEndOfFrame();
            }

            // Load debug assemblies if debugging is enabled
            if (LoaderManager.Instance.IsDebuggingEnabled)
            {
                var localModAssemblies = LocalMods.GetLocalModDebugAssemblies();

                foreach (var debugAssembly in localModAssemblies)
                {
                    Debug.Log($"Loading plugin debug assembly '{debugAssembly}'");

                    var fileName = Path.GetFileNameWithoutExtension(debugAssembly);
                    LoadPlugin(fileName, debugAssembly);
                }
            }

            Debug.Log($"Loaded {_plugins.Count} plugins");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            // Cleanup
            UnloadAllPlugins();
        }

        /// <summary>
        ///     Loads plugin from given plugin assembly file.
        /// </summary>
        /// <param name="addonName">The addon name.</param>
        /// <param name="pluginAssembly">The addon plugin assembly file</param>
        public void LoadPlugin(string addonName, string pluginAssembly)
        {
            if (_plugins.TryGetValue(addonName, out var prevPlugin))
            {
                Debug.LogError("Plugin '" + addonName + "' already loaded!");
                foreach (var prevPluginPlugin in prevPlugin.Plugins)
                {
                    try
                    {
                        prevPluginPlugin.OnUnload();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                _plugins.Remove(addonName);
            }

            // Copy assembly to a temporary directory, as we need to use Assembly.LoadFile to always load a new assembly,
            // in case, the assembly name is the same - all other loading methods would reuse the assembly that is
            // already loaded in the memory.
            
            var tempPluginFile = Path.GetFileNameWithoutExtension(pluginAssembly) + Guid.NewGuid() + ".dll";
            var tempPluginPath = Path.GetFullPath("AddonManager/AddonsCacheTemp/" + tempPluginFile);
            File.Copy(pluginAssembly, tempPluginPath);

            var assembly = Assembly.LoadFile(tempPluginPath);
            Debug.Log($"Plugin assembly {pluginAssembly} loaded from {tempPluginPath}");

            var plugins = new List<IPlugin>();
            // TODO: Maybe we do not want to allow multiple plugins per addon...?
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    try
                    {
                        var instance = (IPlugin)Activator.CreateInstance(type);
                        instance.OnLoad();
                        plugins.Add(instance);

                        Debug.Log("Activated plugin " + type);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            // Add plugin info to dict
            _plugins.Add(addonName, new PluginInfo
            {
                Assembly = assembly,
                Plugins = plugins.ToArray()
            });
        }

        /// <summary>
        ///     Unloads all plugins that has been loaded.
        /// </summary>
        public void UnloadAllPlugins()
        {
            foreach (var plugin in _plugins)
            {
                UnloadPlugin(plugin.Value);
            }

            // Clear the plugin dictionary
            _plugins.Clear();
        }

        private void UnloadPlugin(PluginInfo pluginInfo)
        {
            foreach (var plugin in pluginInfo.Plugins)
                plugin.OnUnload();
        }
    }
}
