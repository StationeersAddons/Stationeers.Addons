// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Linq;
using Stationeers.Addons.Core;
using UnityEngine;

namespace Stationeers.Addons.Modules.LiveReload
{
    /// <summary>
    ///     HarmonyLib module. Provides support for dynamic plugin reload at runtime for debug purposes.
    /// </summary>
    internal class LiveReloadModule : IModule
    {
        /// <inheritdoc />
        public string LoadingCaption => "Initializing live reload module...";
        
        private bool _liveReloadEnabled;

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            _liveReloadEnabled = System.Environment.GetCommandLineArgs().Any(a => a == "--live-reload");
            if (_liveReloadEnabled)
                Debug.Log("[Stationeers.Addons] Live reload enabled! Press CTRL+R to reload all plugins.");
            
            yield break;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
        }

        /// <summary>
        ///     Updates the LiveReload module.
        /// </summary>
        public void Update()
        {
            if (!_liveReloadEnabled) return;
            
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
            {
                LoaderManager.Instance.StartCoroutine(LoaderManager.Instance.Reload());
            }
        }
    }
}