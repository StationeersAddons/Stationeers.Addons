// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.Modules.Workshop
{
    /// <summary>
    ///     Class containing methods for workshop upload patches (PublishWorkshopPrefix, OnSubmitItemUpdatePostfix).
    /// </summary>
    public static class WorkshopManagerPatch
    {
        /*private static readonly Regex[] ValidFileNames =
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

        [UsedImplicitly]
        public static void PublishWorkshopPrefix(WorkshopManager __instance, ref WorkShopItemDetail ItemDetail, ref string changeNote)
        {
            var origItemContentPath = ItemDetail.Path;
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
            ItemDetail.Path = tempItemContentPath;

            Debug.Log("Created temporary workshop item directory " + tempItemContentPath);
        }

        [UsedImplicitly]
        public static void OnSubmitItemUpdatePostfix(WorkshopManager __instance, SteamAsyncSubmitItemUpdate Parent, bool WasSuccessful, WorkShopItemDetail ItemDetail, bool UserNeedsToAcceptWorkshopLegalAgreement)
        {
            var tempItemContentPath = ItemDetail.Path;
            var origItemContentPath = tempItemContentPath.Replace("_temp", "");

            Debug.Log("Checking for temporary workshop item directory " + tempItemContentPath);
            if (Directory.Exists(tempItemContentPath) && tempItemContentPath.Contains("_temp"))
            {
                // Recursively remove the temp dir after steam is done with it.
                Debug.Log("Cleared temporary workshop item directory " + tempItemContentPath);
                Directory.Delete(tempItemContentPath, true);
            }

            // Put things back how we found them
            ItemDetail.Path = origItemContentPath;
        }*/
    }
}
