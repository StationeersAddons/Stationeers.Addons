// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Linq;
using Assets.Scripts.UI;
using ImGuiNET;
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
            ImGuiUn.Layout += OnLayout;
        }

        private void OnLayout()
        {
            if (!_isRecompiling)
                return;

            ImGuiLoadingScreen.ShowLoadingScreen(null, ImGuiLoadingScreen.Singleton.State, 0.1f);
        }

        /// <inheritdoc />
        public IEnumerator Load()
        {
            _liveReloadEnabled = System.Environment.GetCommandLineArgs().Any(a => a == "--live-reload");
            if (_liveReloadEnabled)
                AddonsLogger.Log("Live reload enabled! Press CTRL+R to reload all plugins.");
            
            yield break;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            ImGuiUn.Layout -= OnLayout;
        }

        /// <summary>
        ///     Updates the LiveReload module.
        /// </summary>
        public void Update()
        {
            if (!_liveReloadEnabled || !LoaderManager.Instance.IsLoaded) return;
            
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
            {
                LoaderManager.Instance.StartCoroutine(Reload());
            }
        }

        internal IEnumerator Reload()
        {
            if (_isRecompiling)
            {
                AddonsLogger.Warning("Already recompiling!");
                yield break;
            }

            _isRecompiling = true;
            
            // Wait a frame
            yield return null;

            // Update caption
            var uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Unloading plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.1f);
            yield return uniTask;

            // Make sure that we show the progress bar
            yield return new WaitForSeconds(0.1f);
            
            AddonsLogger.Log("Unloading plugins");
            
            // Reinitialize Harmony, so we can call Load method again later to patch the game again
            LoaderManager.Instance.Harmony.Shutdown();
            LoaderManager.Instance.Harmony.Initialize();
            LoaderManager.Instance.PluginLoader.UnloadAllPlugins();

            uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Recompiling plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.25f);
            yield return uniTask;
            
            AddonsLogger.Log("Recompiling plugins");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.PluginCompiler.Load();

            uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Reloading plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.50f);
            yield return uniTask;
            
            AddonsLogger.Log("Reloading plugins");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.PluginLoader.Load();

            uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Patching plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.50f);
            yield return uniTask;

            AddonsLogger.Log("Re-patching game using harmony");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.Harmony.Load();
            
            AddonsLogger.Log("Recompilation done");
            
            _isRecompiling = false;
        }
    }
}