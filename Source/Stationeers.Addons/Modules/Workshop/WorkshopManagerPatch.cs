using Assets.Scripts;
using Assets.Scripts.Steam;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Stationeers.Addons.Modules.Workshop
{
    public class WorkshopManagerPatch
    {
        // Why does System.IO not have a directory copy method?
        // Stolen from here: https://stackoverflow.com/questions/1974019/folder-copy-in-c-sharp
        private static void CopyDirectory(string strSource, string strDestination)
        {
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
            }

            var dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
            }

            DirectoryInfo[] directories = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in directories)
            {
                CopyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
            }

        }

        public static void PublishWorkshopPrefix(WorkshopManager __instance, ref WorkShopItemDetail ItemDetail, ref string changeNote)
        {
            string origItemContentPath = ItemDetail.Path;
            string tempItemContentPath = origItemContentPath + "_temp";

            if (!Directory.Exists(tempItemContentPath))
            {
                Directory.CreateDirectory(tempItemContentPath);
            }

            // Copy files to upload to temp directory, if they satisfy upload whitelist.
            foreach (string itemFilePath in Directory.GetFiles(origItemContentPath))
            {
                string fileName = new FileInfo(itemFilePath).Name;

                var validFile = ValidFileNames.Any(regex => regex.IsMatch(fileName));

                if (validFile)
                    File.Copy(itemFilePath, tempItemContentPath + Path.GetFileName(itemFilePath));
            }

            // Copy directories to upload to temp directory, if they satisfy upload whitelist.
            foreach (string itemFolderPath in Directory.GetDirectories(origItemContentPath))
            {
                string dirName = new FileInfo(itemFolderPath).Name;

                var validDir = ValidDirectoryNames.Any(regex => regex.IsMatch(dirName));

                if (validDir)
                    CopyDirectory(itemFolderPath, tempItemContentPath + Path.DirectorySeparatorChar + dirName);
            }

            // Set workshop item info to use temporary path before ISteamUCG gets its hands on it.
            ItemDetail.Path = tempItemContentPath;

            Debug.Log("Created temporary workshop item directory " + tempItemContentPath);
        }

        public static void SubmitItemUpdatePostfix(WorkshopManager __instance, UGCUpdateHandle_t UGCUpdateHandle, WorkShopItemDetail ItemDetail)
        {
            string tempItemContentPath = ItemDetail.Path;
            string origItemContentPath = tempItemContentPath.Replace("_temp", "");

            Debug.Log("Checking for temporary workshop item directory " + tempItemContentPath);
            if (Directory.Exists(tempItemContentPath) && tempItemContentPath.Contains("_temp"))
            {
                // Recursively remove the temp dir after steam is done with it.
                Debug.Log("Cleared temporary workshop item directory " + tempItemContentPath);
                Directory.Delete(tempItemContentPath, true);
            }

            // Put things back how we found them
            ItemDetail.Path = origItemContentPath;
        }

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
    }
}
