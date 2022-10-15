// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Assets.Scripts.Networking.Transports;
using JetBrains.Annotations;
using Stationeers.Addons.Core;
using Stationeers.Addons.Utilities;
using UnityEngine.UI;

namespace Stationeers.Addons.Modules.Workshop
{
    /// <summary>
    ///     Class containing methods for workshop upload patch.
    /// </summary>
    internal static class WorkshopManagerPatch
    {
        private static readonly Regex[] ValidFileNames =
        {
            new Regex(@".*\.cs$"),
            new Regex(@".*\.xml$"),
            new Regex(@".*\.png$"),
            new Regex(@".*\.asset$"),
            new Regex(@"^LICENSE$")
        };

        private static readonly Regex[] ValidDirectoryNames =
        {
            new Regex(@"^About$"),
            new Regex(@"^Content$"),
            new Regex(@"^GameData$"),
            new Regex(@"^Scripts$")
        };

        private static WorkshopModListItem _currentMod;

        private static WorkshopModListItem GetSelectedModData(WorkshopMenu inst)
        {
            // Use reflection to read _selectedModItem field from inst
            var selectedModItem = inst.GetType().GetField("_selectedModItem", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(inst);
            return (WorkshopModListItem) selectedModItem;
        }

        [UsedImplicitly]
        public static void RefreshButtonsPostfix(WorkshopMenu __instance)
        {
            var selectedMod = GetSelectedModData(__instance);
            
            if (!selectedMod.Data.IsLocal) return;
            
            __instance.SelectedModButtonRight.GetComponent<Button>().onClick.RemoveAllListeners();
            __instance.SelectedModButtonRight.GetComponent<Button>().onClick.AddListener(() => PublishModOverride(__instance));
        }
        
        private static void PublishModOverride(WorkshopMenu instance)
        {
            _currentMod = GetSelectedModData(instance);
            var modData = _currentMod.Data;
            
            AddonsLogger.Log($"Publishing mod '{modData.GetAboutData().Name}' using Stationeers.Addon filter");
            
            var origItemContentPath = modData.LocalPath;
            var tempItemContentPath = origItemContentPath + "_temp";

            if (Directory.Exists(tempItemContentPath))
            {
                Directory.CreateDirectory(tempItemContentPath);
            }

            // Copy files to upload to temp directory, if they satisfy upload whitelist.
            foreach (var itemFilePath in Directory.GetFiles(origItemContentPath))
            {
                var fileName = new FileInfo(itemFilePath).Name;

                var validFile = ValidFileNames.Any(regex => regex.IsMatch(fileName));

                if (validFile)
                    File.Copy(itemFilePath, tempItemContentPath + Path.GetFileName(itemFilePath), true);
            }

            // Copy directories to upload to temp directory, if they satisfy upload whitelist.
            foreach (var itemFolderPath in Directory.GetDirectories(origItemContentPath))
            {
                var dirName = new FileInfo(itemFolderPath).Name;

                var validDir = ValidDirectoryNames.Any(regex => regex.IsMatch(dirName));

                if (validDir)
                    DirectoryEx.Copy(itemFolderPath, tempItemContentPath + Path.DirectorySeparatorChar + dirName);
            }

            modData.LocalPath = tempItemContentPath;
            _currentMod.SetData(modData);

            AddonsLogger.Log("Created temporary workshop item directory " + tempItemContentPath);
            
            Publish();
        }

        private static async void Publish()
        {
            var mod = _currentMod.Data;
            var aboutData = mod.GetAboutData();
            var localPath = mod.LocalPath;

            AddonsLogger.Log("Uploading mod from " + localPath);
            
            var itemDetail = new SteamTransport.WorkShopItemDetail
            {
                Title = aboutData.Name,
                Path = localPath,
                PreviewPath = localPath + "\\About\\thumb.png",
                Description = aboutData.Description,
                PublishedFileId = aboutData.WorkshopHandle,
                Type = SteamTransport.WorkshopType.Mod,
                CustomTags = aboutData.Tags
            };
            
            var (success, fileId) = await SteamTransport.Workshop_PublishItemAsync(itemDetail);

            if (!success)
            {
                AddonsLogger.Error("Failed to publish mod to Steam Workshop! If error is 'FileNotFound', " +
                                   "mod has been deleted from workshop or you do not have access to it." +
                                   "Remove WorkshopHandle tag from About.xml file");
                Cleanup();
                return;
            }

            // Cleanup now.
            // We have to restore LocalPath to allow SaveWorkShopFileHandle method to store workshop file id.
            Cleanup();
            
            itemDetail.PublishedFileId = fileId;

            var saveMethod = typeof(WorkshopMenu).GetMethod("SaveWorkShopFileHandle", BindingFlags.Static | BindingFlags.NonPublic);
            
            if (saveMethod == null)
            {
                AddonsLogger.Error("Failed to invoke 'SaveWorkShopFileHandle' method!");
            }
            else
            {
                saveMethod.Invoke(null, new object[] { itemDetail, mod });
            }
        }

        private static void Cleanup()
        {
            var modData = _currentMod.Data;
            var tempItemContentPath = modData.LocalPath;
            var origItemContentPath = tempItemContentPath.Replace("_temp", "");

            AddonsLogger.Log("Checking for temporary workshop item directory " + tempItemContentPath);
            if (Directory.Exists(tempItemContentPath) && tempItemContentPath.Contains("_temp"))
            {
                // Recursively remove the temp dir after steam is done with it.
                AddonsLogger.Log("Cleared temporary workshop item directory " + tempItemContentPath);
                Directory.Delete(tempItemContentPath, true);
            }

            // Put things back how we found them
            modData.LocalPath = origItemContentPath;
            _currentMod.SetData(modData);
        }
    }
}
