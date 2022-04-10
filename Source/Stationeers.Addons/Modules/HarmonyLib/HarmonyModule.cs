// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
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

        private Harmony _harmony;

        /// <inheritdoc />
        public void Initialize()
        {
            _harmony = new Harmony("com.stationeers.addons");
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            Debug.Log("Patching WorkshopManager using Harmony...");
            try
            {
                // var publishWorkshopMethod = typeof(WorkshopManager).GetMethod("PublishWorkshop", BindingFlags.Public | BindingFlags.Instance);
                // var onSubmitItemUpdateMethod = typeof(WorkshopManager).GetMethod("OnSubmitItemUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
                // var publishWorkshopPrefixMethod = typeof(WorkshopManagerPatch).GetMethod("PublishWorkshopPrefix");
                // var onSubmitItemUpdatePostfixMethod = typeof(WorkshopManagerPatch).GetMethod("OnSubmitItemUpdatePostfix");
                //
                // _harmony.Patch(publishWorkshopMethod, new HarmonyMethod(publishWorkshopPrefixMethod));
                // _harmony.Patch(onSubmitItemUpdateMethod, null, new HarmonyMethod(onSubmitItemUpdatePostfixMethod));
            } 
            catch (Exception ex)
            {
                AlertPanel.Instance.ShowAlert($"Failed to initialize workshop publish patch!\n", AlertState.Alert);
                Debug.LogError($"Failed to initialize workshop publish patch. Exception:\n{ex}");
            }

            Debug.Log("Patching game assembly using Harmony...");
            foreach (var plugin in LoaderManager.Instance.PluginLoader.LoadedPlugins)
            {
                Debug.Log($"Applying patches from assembly '{plugin.Assembly.FullName}'");
                _harmony.PatchAll(plugin.Assembly);
                yield return new WaitForEndOfFrame();
            }
            Debug.Log($"Finished {LoaderManager.Instance.PluginLoader.LoadedPlugins.Count} patches to game assembly");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _harmony.UnpatchAll();
            _harmony = null;
        }
    }
}
