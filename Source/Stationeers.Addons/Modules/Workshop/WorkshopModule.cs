// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using Assets.Scripts.UI;
using HarmonyLib;
using Stationeers.Addons.Core;
using Stationeers.Addons.Modules.HarmonyLib;

namespace Stationeers.Addons.Modules.Workshop
{
    /// <summary>
    ///     WorkshopModule module class. This is required,
    ///     because we're compiling plugins only from the workshop,
    ///     and we need a list of subscribed items for that.
    /// </summary>
    internal class WorkshopModule : IModule
    {
        public string LoadingCaption => "Loading Workshop...";

        /// <inheritdoc />
        public void Initialize()
        {
            // Nothing to initialize
            HarmonyModule.RegisterPatcher(OnHarmonyPatch);
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            yield return null;
            AddonsLogger.Log("Workshop loaded!");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            // Cleanup?
        }

        private void OnHarmonyPatch(Harmony harmony)
        {
            AddonsLogger.Log("Patching WorkshopManager using Harmony...");
            try
            {
                var refreshButtonsMethod = typeof(WorkshopMenu).GetMethod("RefreshButtons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var refreshButtonsPostfixPrefix = typeof(WorkshopManagerPatch).GetMethod("RefreshButtonsPostfix");
                harmony.Patch(refreshButtonsMethod, null, new HarmonyMethod(refreshButtonsPostfixPrefix));
            }
            catch (Exception ex)
            {
                //AlertPanel.Instance.ShowAlert($"Failed to initialize workshop publish patch!\n", AlertState.Alert); // TODO
                AddonsLogger.Error($"Failed to initialize workshop publish patch. Exception:\n{ex}");
            }

        }
    }
}
