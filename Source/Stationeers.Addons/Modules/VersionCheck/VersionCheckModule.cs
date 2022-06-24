// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.UI;
using UI.ImGuiUi;
using UnityEngine;
using UnityEngine.Networking;

namespace Stationeers.Addons.Modules.Plugins
{
    internal class VersionCheckModule : IModule
    {
        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public IEnumerator Load()
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
                    
                    // TODO: Figure out how to display alerts as devs broke the AlertPanel.Instance again...
                    ConsoleWindow.Print($"New version of Stationeers.Addons ({data}) is available!\n", ConsoleColor.Red);
                }
                else
                {
                    Debug.LogError(
                        $"Failed to request latest Stationeers.Addons version. Error: '\"{webRequest.error}\""
                    );
                    ConsoleWindow.PrintError("Failed to check latest Stationeers.Addons version!\n");

                    // Wait for the alert window to close
                    while (AlertPanel.Instance.AlertWindow.activeInHierarchy)
                        yield return null;
                }
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
        }

        public string LoadingCaption => "Checking for a new version...";
    }
}
