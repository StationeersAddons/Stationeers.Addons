// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Stationeers.Addons.Loader.Modules.Plugins
{
    internal class PluginLoaderModule : IModule
    {
        private struct PluginInfo
        {
            public Assembly Assembly { get; set; }
            public IPlugin[] Plugins { get; set; }
        }

        private readonly Dictionary<string, PluginInfo> _plugins = new Dictionary<string, PluginInfo>();

        public int NumLoadedPlugins => _plugins.Count;
        public string LoadingCaption => "Starting up plugins...";

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            foreach (var compiledPlugin in PluginCompilerModule.CompiledPlugins)
            {
                Debug.Log($"Loading plugin assembly '{compiledPlugin.AssemblyFile}'");
                LoadPlugin(compiledPlugin.AddonName, compiledPlugin.AssemblyFile);
                yield return new WaitForEndOfFrame();
            }
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
            if (_plugins.ContainsKey(addonName))
            {
                Debug.LogError("Plugin '" + addonName + "' already loaded!");
                return;
            }

            var assembly = Assembly.LoadFile(pluginAssembly);

            Debug.Log("Plugin assembly " + pluginAssembly + " loaded ");

            var plugins = new List<IPlugin>();
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    var instance = (IPlugin)Activator.CreateInstance(type);
                    instance.OnLoad();
                    plugins.Add(instance);

                    Debug.Log("Activated plugin " + type);
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
        }

        private void UnloadPlugin(PluginInfo pluginInfo)
        {
            foreach (var plugin in pluginInfo.Plugins)
                plugin.OnUnload();
        }
    }
}
