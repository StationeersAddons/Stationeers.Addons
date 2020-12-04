// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Stationeers.Addons.Modules;
using Stationeers.Addons.Modules.Bundles;
using Stationeers.Addons.Modules.HarmonyLib;
using Stationeers.Addons.Modules.Plugins;
using Stationeers.Addons.Modules.Workshop;
using UnityEngine;

namespace Stationeers.Addons.Core
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

        private readonly List<IModule> _modules = new List<IModule>();

        /// <summary>
        ///     Workshop module reference
        /// </summary>
        public WorkshopModule Workshop { get; private set; }

        /// <summary>
        ///     PluginCompiler module reference
        /// </summary>
        public PluginCompilerModule PluginCompiler { get; private set; }

        /// <summary>
        ///     BundleLoaderModule module reference
        /// </summary>
        public BundleLoaderModule BundleLoader { get; private set; }

        /// <summary>
        ///     PluginLoader module reference
        /// </summary>
        public PluginLoaderModule PluginLoader { get; private set; }

        /// <summary>
        ///     Harmony module reference
        /// </summary>
        public HarmonyModule Harmony { get; private set; }

        public void Activate()
        {
            Debug.Log("ModLoader activated!");
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Workshop = InitializeModule<WorkshopModule>();
            PluginCompiler = InitializeModule<PluginCompilerModule>();
            BundleLoader = InitializeModule<BundleLoaderModule>();
            PluginLoader = InitializeModule<PluginLoaderModule>();
            Harmony = InitializeModule<HarmonyModule>();
        }

        private IEnumerator Start()
        {
            // TODO: Start loading only when the initial main menu loading is finished

            ProgressPanel.Instance.ShowProgressBar("<b>Stationeers.Addons</b>");
            ProgressPanel.Instance.UpdateProgressBarCaption("Loading modules...");
            ProgressPanel.Instance.UpdateProgressBar(0.1f);

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
            GUI.Label(new Rect(5.0f, 5.0f, Screen.width, 25.0f), $"Stationeers.Addons - v0.1.0 - Loaded {PluginLoader.NumLoadedPlugins} plugins");
        }

        private TModuleType InitializeModule<TModuleType>() where TModuleType : IModule, new()
        {
            var moduleInstance = new TModuleType();
            _modules.Add(moduleInstance);
            moduleInstance.Initialize();
            return moduleInstance;
        }
    }
}