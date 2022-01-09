// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Linq;
using Assets.Scripts.UI;
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
        private bool _isRecompiling;

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
                LoaderManager.Instance.StartCoroutine(Reload());
            }
        }

        internal IEnumerator Reload()
        {
            if (_isRecompiling)
            {
                Debug.LogWarning("Already recompiling!");
                yield break;
            }
            
            _isRecompiling = true;

            ProgressPanel.Instance.ShowProgressBar("<b>Stationeers.Addons</b>");
            ProgressPanel.Instance.UpdateProgressBarCaption("Live Reload - Unloading plugins...");
            ProgressPanel.Instance.UpdateProgressBar(0.1f);

            // Make sure that we show the progress bar
            yield return new WaitForSeconds(0.1f);
            
            Debug.Log("Unloading plugins");
            LoaderManager.Instance.PluginLoader.UnloadAllPlugins();

            ProgressPanel.Instance.UpdateProgressBarCaption("Live Reload - Recompiling plugins...");
            ProgressPanel.Instance.UpdateProgressBar(0.25f);
            
            Debug.Log("Recompiling plugins");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.PluginCompiler.Load();

            ProgressPanel.Instance.UpdateProgressBarCaption("Live Reload - Reloading plugins...");
            ProgressPanel.Instance.UpdateProgressBar(0.75f);
            
            Debug.Log("Reloading plugins");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.PluginLoader.Load();
            
            Debug.Log("Recompilation done");

            ProgressPanel.Instance.HideProgressBar();
            
            _isRecompiling = false;
        }
    }
}