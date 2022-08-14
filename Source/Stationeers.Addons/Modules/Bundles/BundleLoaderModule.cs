// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.IO;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Transports;
using Stationeers.Addons.API;
using Stationeers.Addons.Core;
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
            AddonsLogger.Log("Loading custom content bundles...");

            var query = NetworkManager.GetLocalAndWorkshopItems(SteamTransport.WorkshopType.Mod).GetAwaiter();

            while (!query.IsCompleted) // This is not how UniTask should be used, but it works for now.
                yield return null;

            var result = query.GetResult();

            foreach (var itemWrap in result)
            {
                var modDirectory = itemWrap.DirectoryPath;
                if (modDirectory == null) {
                    AddonsLogger.Warning($"Missing mod directory for mod with id={itemWrap.Id}");
                    yield break;
                }
                yield return LoadBundleFromModDirectory(modDirectory);
            }
        }

        private IEnumerator LoadBundleFromModDirectory(string modDirectory)
        {
            var contentDirectory = Path.Combine(modDirectory, "Content");

            // Skip if this mod does not have any custom content
            if (!Directory.Exists(contentDirectory))  yield break;

            AddonsLogger.Log(contentDirectory);

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

                AddonsLogger.Log($"Loaded asset bundle '{bundle.assetBundle.name}'");

                // We're done, just register the bundle and register it.
                BundleManager.LoadedAssetBundles.Add(bundle.assetBundle);
            }
        }

        public void Shutdown()
        {
            AddonsLogger.Log("Unloading all custom content bundles...");

            // Unload all loaded asset bundles
            foreach (var bundle in BundleManager.LoadedAssetBundles)
            {
                bundle.Unload(true);
            }
        }
    }
}
