// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using Stationeers.Addons.Core;
using UnityEngine;

/*
 * DO NOT MODIFY THIS FILE, unless you know what you are doing.
 * Loader class is being used as an entry point of the addons loader.
 * We cannot change access modifiers, names or types.
 */

namespace Stationeers.Addons
{
    public class Loader
    {
        // ReSharper disable once UnusedMember.Local
        public void Load()
        {
            Debug.Log("Hello, World!");

            // Create instance of ModLoader
            LoaderManager.Instance.Activate();
        }
    }
}