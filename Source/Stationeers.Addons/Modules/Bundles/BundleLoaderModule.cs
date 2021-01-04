// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.IO;
using Assets.Scripts;
using Stationeers.Addons.API;
using Stationeers.Addons.Core;
using Steamworks;
using UnityEngine;

namespace Stationeers.Addons.Modules.Bundles
{
    /// <summary>
    ///     BundleModule. Provides asset bundle loading support.
    /// </summary>
    internal class BundleLoaderModule : IModule
    {
        /// <inheritdoc />
        public string LoadingCaption => "Loading custom content...";

        public void Initialize()
        {
        }

        public IEnumerator Load()
        {
            Debug.Log("Loading custom content bundles...");

            foreach (var workshopItemID in WorkshopManager.Instance.SubscribedItems)
            {
                // TODO: Read XML file and get the real addon name to show

                if (SteamUGC.GetItemInstallInfo(workshopItemID, out _, out var pchFolder, 1024U, out _))
                {
                    // TODO: Prevent from loading local addons

                    var modDirectory = pchFolder;
                    yield return LoadBundleFromModDirectory(modDirectory);
                }
            }

            // Load debug assemblies if debugging is enabled
            if (LoaderManager.Instance.IsDebuggingEnabled)
            {
                var localModDirectories = LocalMods.GetLocalModDirectories();

                foreach (var localModDirectory in localModDirectories)
                {
                    yield return LoadBundleFromModDirectory(localModDirectory);
                }
            }
        }

        private IEnumerator LoadBundleFromModDirectory(string modDirectory)
        {
            var contentDirectory = Path.Combine(modDirectory, "Content");

            // Skip if this mod does not have any custom content
            if (!Directory.Exists(contentDirectory))  yield break;

            // Get all bundle files
            var bundles = Directory.GetFiles(contentDirectory, "*.asset", SearchOption.TopDirectoryOnly);

            // If no bundles were found, skip.
            if (bundles.Length == 0) yield break;

            foreach (var bundleFile in bundles)
            {
                // Start loading the bundle
                var bundle = AssetBundle.LoadFromFileAsync(bundleFile);

                // Wait for bundle to load
                yield return bundle;

                Debug.Log($"Loaded asset bundle '{bundle.assetBundle.name}'");

                // We're done, just register the bundle and register it.
                BundleManager.LoadedAssetBundles.Add(bundle.assetBundle);
            }
        }

        public void Shutdown()
        {
            Debug.Log("Unloading all custom content bundles...");

            // Unload all loaded asset bundles
            foreach (var bundle in BundleManager.LoadedAssetBundles)
            {
                bundle.Unload(true);
            }
        }
    }
}
