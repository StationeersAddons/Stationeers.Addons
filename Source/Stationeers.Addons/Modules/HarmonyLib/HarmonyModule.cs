// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using HarmonyLib;
using Stationeers.Addons.Core;
using UnityEngine;

namespace Stationeers.Addons.Modules.HarmonyLib
{
    /// <summary>
    ///     HarmonyLib module. Provides support for dynamic game assembly patching.
    /// </summary>
    internal class HarmonyModule : IModule
    {
        public string LoadingCaption => "Initializing Harmony library...";

        private Harmony _harmony;

        /// <inheritdoc />
        public void Initialize()
        {
            _harmony = new Harmony("com.stationeers.addons");
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            Debug.Log("Patching game assembly using Harmony...");
            foreach (var plugin in LoaderManager.Instance.PluginLoader.LoadedPlugins)
            {
                Debug.Log($"Applying patches from assembly '{plugin.Assembly.FullName}'");
                _harmony.PatchAll(plugin.Assembly);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _harmony.UnpatchAll();
            _harmony = null;
        }
    }
}
