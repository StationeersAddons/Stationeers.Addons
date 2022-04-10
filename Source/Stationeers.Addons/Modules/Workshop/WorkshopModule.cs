// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using Assets.Scripts;
using UnityEngine;

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
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            yield return null;
            // Start loading workshop
            // WorkshopManager.Instance.LoadWorkshopItems();
            // WorkshopManager.Instance.GetSubscribedItems();
            //
            // // Hack to detect when items are loaded.
            // // Will be created inside steam callback
            // WorkshopManager.Instance.SubscribedItems = null;
            //
            // // Wait until we get workshop loaded
            // while (WorkshopManager.Instance.SubscribedItems == null)
            //     yield return null;

            Debug.Log("Workshop loaded!");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            // Cleanup?
        }
    }
}
