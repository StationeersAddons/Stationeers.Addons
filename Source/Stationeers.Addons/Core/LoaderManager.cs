// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.UI;
using Stationeers.Addons.Modules;
using Stationeers.Addons.Modules.Bundles;
using Stationeers.Addons.Modules.HarmonyLib;
using Stationeers.Addons.Modules.LiveReload;
using Stationeers.Addons.Modules.Plugins;
using Stationeers.Addons.Modules.Workshop;
using UnityEngine;
using UnityEngine.Networking;

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
        ///     Gets true when the debugging has been enabled.
        /// </summary>
        public bool IsDebuggingEnabled { get; private set; }

        /// <summary>
        ///     Gets true when running on a dedicated server
        /// </summary>
        public static bool IsDedicatedServer { get; private set; }

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

        /// <summary>
        ///     LiveReload module reference
        /// </summary>
        public LiveReloadModule LiveReload { get; private set; }

        private bool _isRecompiling;

        public void Activate()
        {
            Debug.Log("ModLoader activated!");
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // Show stacktrace only when it's exception or assert
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

            // Check if we are running on a dedicated server instance
            IsDedicatedServer = File.Exists("rocketstation_DedicatedServer.exe"); // TODO: Executable file name for Linux dedicated server
            if (IsDedicatedServer)
                Debug.Log("[Stationeers.Addons - DEDICATED SERVER] Stationeers.Addons is running on a dedicated server!");

            // Check if we can debug addons
            IsDebuggingEnabled = File.Exists("addons-debugging.enable");
            if (IsDebuggingEnabled)
                Debug.Log("[Stationeers.Addons - DEBUG] Stationeers.Addons debugging enabled!");

            if(!IsDedicatedServer) // Only look for workshop if on client
                Workshop = InitializeModule<WorkshopModule>();
            
            PluginCompiler = InitializeModule<PluginCompilerModule>();
            BundleLoader = InitializeModule<BundleLoaderModule>();
            PluginLoader = InitializeModule<PluginLoaderModule>();
            Harmony = InitializeModule<HarmonyModule>();
            LiveReload = InitializeModule<LiveReloadModule>();
        }

        private IEnumerator Start()
        {
            if(!IsDedicatedServer)
            {
                // Check version
                yield return CheckVersion();
                
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
            else // Remove UI elements for dedicated server loading
            {
                foreach (var module in _modules)
                {
                    yield return module.Load();
                }
            }
        }

        private void Update()
        {
            LiveReload?.Update();
        }

        internal IEnumerator Reload()
        {
            if (_isRecompiling)
            {
                Debug.LogWarning("Already recompiling!");
                yield break;
            }
            
            _isRecompiling = true;
            
            Debug.Log("Unloading plugins");
            PluginLoader.UnloadAllPlugins();
            
            Debug.Log("Recompiling plugins");
            yield return PluginCompiler.Load();
            
            Debug.Log("Reloading plugins");
            yield return PluginLoader.Load();
            
            Debug.Log("Recompilation done");
            
            _isRecompiling = false;
        }
        
        private IEnumerator CheckVersion()
        {
            Debug.Log("Checking for Stationeers.Addons version...");
            
            // Perform simple web request to get the latest version from github
            using (var webRequest = UnityWebRequest.Get(Globals.VersionFile))
            {
                yield return webRequest.SendWebRequest();

                if (!webRequest.isHttpError && !webRequest.isNetworkError)
                {
                    var data = webRequest.downloadHandler.text.Trim();
                    
                    Debug.Log($"Latest Stationeers.Addons version is {data}. Installed {Globals.Version}");
                    
                    // If the current version is the same as the latest one, just exit the coroutine.
                    if (Globals.Version == data)
                        yield break;
                    
                    Debug.Log("New version of Stationeers.Addons is available!");
                    
                    while (LoadingPanel.Instance.IsVisible)
                        yield return null;
                        
                    AlertPanel.Instance.ShowAlert($"New version of Stationeers.Addons ({data}) is available!\n", 
                        AlertState.Alert);
                        
                    // Wait for the alert window to close
                    while (AlertPanel.Instance.AlertWindow.activeInHierarchy)
                        yield return null;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var module in _modules)
                module.Shutdown();
        }

        private void OnGUI()
        {
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.15f);
            GUI.Label(new Rect(5.0f, 5.0f, Screen.width, 25.0f),
                _isRecompiling
                    ? $"Stationeers.Addons - {Globals.Version} - Recompiling plugins"
                    : $"Stationeers.Addons - {Globals.Version} - Loaded {PluginLoader.NumLoadedPlugins} plugins");
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
