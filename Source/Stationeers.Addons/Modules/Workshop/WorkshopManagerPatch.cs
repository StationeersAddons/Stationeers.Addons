// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Stationeers.Addons.Core;
using Stationeers.Addons.Utilities;

namespace Stationeers.Addons.Modules.Workshop
{
    /// <summary>
    ///     Class containing methods for workshop upload patches (PublishModPrefix, OnSubmitItemUpdatePostfix).
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

        private static ModData GetSelectedModData(WorkshopMenu inst)
        {
            // Use reflection to read _selectedModItem field from inst
            var selectedModItem = inst.GetType().GetField("_selectedModItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(inst);
            return (ModData) selectedModItem;
        }

        [UsedImplicitly]
        public static void PublishModPrefix(WorkshopMenu __instance)
        {
            var modData = GetSelectedModData(__instance);
            
            var origItemContentPath = modData.LocalPath;
            var tempItemContentPath = origItemContentPath + "_temp";

            if (!Directory.Exists(tempItemContentPath))
            {
                Directory.CreateDirectory(tempItemContentPath);
            }

            // Copy files to upload to temp directory, if they satisfy upload whitelist.
            foreach (var itemFilePath in Directory.GetFiles(origItemContentPath))
            {
                var fileName = new FileInfo(itemFilePath).Name;

                var validFile = ValidFileNames.Any(regex => regex.IsMatch(fileName));

                if (validFile)
                    File.Copy(itemFilePath, tempItemContentPath + Path.GetFileName(itemFilePath));
            }

            // Copy directories to upload to temp directory, if they satisfy upload whitelist.
            foreach (var itemFolderPath in Directory.GetDirectories(origItemContentPath))
            {
                var dirName = new FileInfo(itemFolderPath).Name;

                var validDir = ValidDirectoryNames.Any(regex => regex.IsMatch(dirName));

                if (validDir)
                    DirectoryEx.Copy(itemFolderPath, tempItemContentPath + Path.DirectorySeparatorChar + dirName);
            }

            // Set workshop item info to use temporary path before ISteamUCG gets its hands on it.
            modData.LocalPath = tempItemContentPath;

            AddonsLogger.Log("Created temporary workshop item directory " + tempItemContentPath);
        }

        [UsedImplicitly]
        public static void PublishModPostfix(WorkshopMenu __instance)
        {
            var modData = GetSelectedModData(__instance);
            
            // _selectedModItem.Data
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
        }
    }
}
