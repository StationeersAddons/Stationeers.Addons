// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
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

        private static List<Action<Harmony>> _patchers = new List<Action<Harmony>>();
        private Harmony _harmony;

        /// <inheritdoc />
        public void Initialize()
        {
            _harmony = new Harmony("com.stationeers.addons");
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            AddonsLogger.Log("Patching game assembly using Harmony...");
            foreach (var plugin in LoaderManager.Instance.PluginLoader.LoadedPlugins)
            {
                AddonsLogger.Log($"Applying patches from assembly '{plugin.Assembly.FullName}'");
                _harmony.PatchAll(plugin.Assembly);
                yield return new WaitForEndOfFrame();
            }

            foreach (var patcher in _patchers)
            {
                AddonsLogger.Log($"Applying patches from assembly '{patcher.Target.GetType().Name}'");
                patcher(_harmony);
            }
            
            AddonsLogger.Log($"Finished {LoaderManager.Instance.PluginLoader.LoadedPlugins.Count} patches to game assembly");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _harmony.UnpatchSelf();
            _harmony = null;
        }
        
        public static void RegisterPatcher(Action<Harmony> patcher)
        {
            _patchers.Add(patcher);
        }
    }
}
