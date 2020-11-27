// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Stationeers.Addons.Loader.Modules;
using Stationeers.Addons.Loader.Modules.Plugins;
using Stationeers.Addons.Loader.Modules.Workshop;
using UnityEngine;

namespace Stationeers.Addons.Loader.Core
{
    /// <summary>
    ///     ModManager class. Implements loader base component.
    /// </summary>
    internal class LoaderManager : MonoBehaviour
    {
        private static LoaderManager _instance;

        /// <summary>
        ///     LoaderManager's instance.
        /// </summary>
        public static LoaderManager Instance
        {
            get
            {
                if (_instance)
                    return _instance;

                var gameObject = new GameObject("ModLoader");
                _instance = gameObject.AddComponent<LoaderManager>();

                return _instance;
            }
        }

        private List<IModule> _modules = new List<IModule>();

        public void Activate()
        {
            Debug.Log("ModLoader activated!");
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _modules.Add(new WorkshopModule());
            _modules.Add(new PluginCompilerModule());
            _modules.Add(new PluginLoaderModule());
            //_modules.Add(new BundleLoaderModule());
        }

        private IEnumerator Start()
        {
            ProgressPanel.Instance.ShowProgressBar("<b>Stationeers.Addons</b>");
            ProgressPanel.Instance.UpdateProgressBarCaption("Initializing modules...");
            ProgressPanel.Instance.UpdateProgressBar(0.1f);

            foreach (var module in _modules)
                module.Initialize();

            var numModules = _modules.Count;
            var moduleIdx = 0;
            foreach (var module in _modules)
            {
                // Calculate modules loading progress
                var progress = Mathf.Clamp01(numModules / (float)moduleIdx);

                // Update caption
                ProgressPanel.Instance.UpdateProgressBarCaption(module.LoadingCaption);
                ProgressPanel.Instance.UpdateProgressBar(Mathf.Lerp(0.35f, 1.0f, progress));

                yield return module.Load();
                moduleIdx++;
            }

            ProgressPanel.Instance.HideProgressBar();
        }

        private void OnDestroy()
        {
            foreach (var module in _modules)
                module.Shutdown();
        }

        private void OnGUI()
        {
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.15f);
            GUI.Label(new Rect(5.0f, 5.0f, Screen.width, 25.0f), $"Stationeers.Addons - v0.1.0 - Loaded {PluginManager.Instance.NumLoadedPlugins} plugins");
        }
    }
}