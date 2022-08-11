// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using Stationeers.Addons.Core;
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
            AddonsLogger.Log("Workshop loaded!");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            // Cleanup?
        }
    }
}
