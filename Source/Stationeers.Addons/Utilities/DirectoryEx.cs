// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.IO;

namespace Stationeers.Addons.Utilities
{
    internal static class DirectoryEx
    {
        /// <summary>
        ///     Copies contents of source into the destination directory.
        /// </summary>
        /// <param name="source">The source directory path.</param>
        /// <param name="destination">The destination directory path (will be created if does not exist).</param>
        public static void Copy(string source, string destination)
        {
            // Source: https://stackoverflow.com/questions/1974019/folder-copy-in-c-sharp

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            var dirInfo = new DirectoryInfo(source);
            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            foreach (var file in files)
            {
                file.CopyTo(Path.Combine(destination, file.Name), true);
            }

            foreach (var tempDirectory in directories)
            {
                Copy(Path.Combine(source, tempDirectory.Name), Path.Combine(destination, tempDirectory.Name));
            }
        }
    }
}