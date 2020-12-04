// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.IO;
using Assets.Scripts;
using Stationeers.Addons.API;
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
                    var addonDirectory = pchFolder;
                    var contentDirectory = Path.Combine(addonDirectory, "Content");

                    // Skip if this mod does not have any custom content
                    if (!Directory.Exists(contentDirectory)) continue;

                    // Get all bundle files
                    var bundles = Directory.GetFiles(contentDirectory, "*.asset", SearchOption.TopDirectoryOnly);

                    // If no bundles were found, skip.
                    if(bundles.Length == 0) continue;

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
