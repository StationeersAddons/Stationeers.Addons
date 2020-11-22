// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using Stationeers.Addons.Loader.Plugins;
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
        ///     PluginManager instance.
        /// </summary>
        public PluginManager PluginManager { get; private set; }

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

        public void Activate()
        {
            Debug.Log("ModLoader activated!");
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            PluginManager = new PluginManager();

            // TODO: BundleManager so we will be able to add new content to the game
        }

        private void OnDestroy()
        {
            // Cleanup
            PluginManager.UnloadAllPlugins();
        }

        private void OnGUI()
        {
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.10f);
            GUI.Label(new Rect(5.0f, 5.0f, Screen.width, 25.0f), "Stationeers.Addons R1 - Loaded 0 addons");
        }
    }
}