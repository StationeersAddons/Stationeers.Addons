// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stationeers.Addons.API
{
    /// <summary>
    ///     AssetManager API class. Provides support for asset bundle lookup.
    /// </summary>
    public static class BundleManager
    {
        internal static readonly List<AssetBundle> LoadedAssetBundles = new List<AssetBundle>();

        /// <summary>
        ///     Looks for asset bundle that has the given <see cref="name"/>.
        /// </summary>
        /// <param name="name">The name of the asset bundle to find.</param>
        /// <returns>The asset bundle. When not found, this will return null!</returns>
        public static AssetBundle GetAssetBundle(string name)
        {
            return LoadedAssetBundles.FirstOrDefault(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
