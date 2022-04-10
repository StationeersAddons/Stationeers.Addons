// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.UI;
using ImGuiNET;
using Stationeers.Addons.Modules;
using Stationeers.Addons.Modules.Bundles;
using Stationeers.Addons.Modules.HarmonyLib;
using Stationeers.Addons.Modules.LiveReload;
using Stationeers.Addons.Modules.Plugins;
using Stationeers.Addons.Modules.Workshop;
using Stationeers.Addons.Utilities;
using UnityEngine;
using Util.Commands;

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
        private bool _isLoading;

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
        ///     VersionCheck module reference
        /// </summary>
        public VersionCheckModule VersionCheck { get; private set; }

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
        
        /// <summary>
        ///     Gets true when the loader is fully loaded.
        /// </summary>
        public bool IsLoaded { get; set; }

        public void Activate()
        {
            Debug.Log($"Stationeers.Addons {Globals.Version}");
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

            VersionCheck = InitializeModule<VersionCheckModule>();
            PluginCompiler = InitializeModule<PluginCompilerModule>();
            BundleLoader = InitializeModule<BundleLoaderModule>();
            PluginLoader = InitializeModule<PluginLoaderModule>();
            Harmony = InitializeModule<HarmonyModule>();
            LiveReload = InitializeModule<LiveReloadModule>();
            
            ImGuiUn.Layout += OnLayout;
        }

        private void OnLayout()
        {
            if (!_isLoading) 
                return;
            
            ImGuiLoadingScreen.ShowLoadingScreen(null, ImGuiLoadingScreen.Singleton.State, 0.1f);
        }

        private IEnumerator Start()
        {
            if(!IsDedicatedServer)
            {
                // Wait for game to fully load into main menu
                // Command line 'CommandLine._processedLaunchArgs' is set after the game has fully loaded (see: WorldManager::LoadGameDataAsync)
                // hacky, but it works.
                while (!(bool)ReflectionHelper.ReadStaticField(typeof(CommandLine), "_processedLaunchArgs"))
                {
                    yield return null;
                }

                _isLoading = true;
                
                // Wait for OnLayout to be called from ImGui
                yield return null;

                var numModules = _modules.Count;
                var moduleIdx = 0;
                foreach (var module in _modules)
                {
                    // Calculate modules loading progress
                    var progress = Mathf.Clamp01(numModules / (float)moduleIdx);

                    // Update caption
                    var uniTask = ImGuiLoadingScreen.Singleton.SetState(module.LoadingCaption);
                    yield return uniTask;
                    
                    uniTask = ImGuiLoadingScreen.Singleton.SetProgress(Mathf.Lerp(0.35f, 1.0f, progress));
                    yield return uniTask;

                    yield return module.Load();
                    moduleIdx++;
                }

                _isLoading = false;
            }
            else // Remove UI elements for dedicated server loading
            {
                foreach (var module in _modules)
                {
                    yield return module.Load();
                }
            }

            IsLoaded = true;
        }

        private void Update()
        {
            LiveReload?.Update();
        }
        
        private void OnDestroy()
        {
            ImGuiUn.Layout -= OnLayout;
            
            foreach (var module in _modules)
                module.Shutdown();
        }

        private void OnGUI()
        {
            // TODO: Maybe port to ImGui?
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.15f);
            GUI.Label(new Rect(5.0f, 5.0f, Screen.width, 25.0f), $"Stationeers.Addons - {Globals.Version} - Loaded {PluginLoader.NumLoadedPlugins} plugins");
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
