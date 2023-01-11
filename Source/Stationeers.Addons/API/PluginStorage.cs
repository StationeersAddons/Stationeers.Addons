// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.IO;
using UnityEngine;

namespace Stationeers.Addons.API
{
    /// <summary>
    ///     PluginStorage class. Allows to read and write serialized structures and binary files.
    /// </summary>
    public static class PluginStorage
    {
        /// <summary>
        ///     Saves a serialized structure to the storage.
        /// </summary>
        /// <param name="data">The object to save.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <typeparam name="TPlugin">The type of the main plugin entry point.</typeparam>
        /// <typeparam name="TType">The type of the serialized structure.</typeparam>
        public static void Save<TPlugin, TType>(TType data, string fileName) 
            where TPlugin : IPlugin 
            where TType : class
        {
            if (!IsFileNameValid(fileName)) throw new Exception("Invalid file name!");
            
            var filePath = GetSaveFilePath<TPlugin>(fileName, "json");
            var jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, jsonData);
        }

        /// <summary>
        ///     Reads a serialized structure from the storage.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <typeparam name="TPlugin">The type of the main plugin entry point.</typeparam>
        /// <typeparam name="TType">The type of the serialized structure.</typeparam>
        /// <returns>The deserialized structure object.</returns>
        public static TType Load<TPlugin, TType>(string fileName)
            where TPlugin : IPlugin
            where TType : class
        {
            if (!IsFileNameValid(fileName)) throw new Exception("Invalid file name!");
            
            var filePath = GetSaveFilePath<TPlugin>(fileName, "json");
            var jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<TType>(jsonData);
        }

        /// <summary>
        ///     Saves a binary file to the storage.
        /// </summary>
        /// <param name="data">The data to save.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <typeparam name="TPlugin">The type of the main plugin entry point.</typeparam>
        public static void Save<TPlugin>(byte[] data, string fileName)
            where TPlugin : IPlugin
        {
            if (!IsFileNameValid(fileName)) throw new Exception("Invalid file name!");
            
            var filePath = GetSaveFilePath<TPlugin>(fileName, "bin");
            File.WriteAllBytes(filePath, data);
        }

        /// <summary>
        ///     Reads a binary file from the storage.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <typeparam name="TPlugin">The type of the main plugin entry point.</typeparam>
        /// <returns>The binary data that is loaded from the given file.</returns>
        public static byte[] Load<TPlugin>(string fileName)
            where TPlugin : IPlugin
        {
            if (!IsFileNameValid(fileName)) throw new Exception("Invalid file name!");
            
            var filePath = GetSaveFilePath<TPlugin>(fileName, "bin");
            return File.ReadAllBytes(filePath);
        }

        private static bool IsFileNameValid(string fileName)
        {
            return fileName.Length > 0 && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
        }
        
        private static string GetBasePluginPath<TPlugin>() 
            where TPlugin : IPlugin
        {
            return Path.Combine(StationSaveUtils.GetSavePath(), "addons_saves", typeof(TPlugin).Name);
        }
        
        private static string GetSaveFilePath<TPlugin>(string fileName, string extension) 
            where TPlugin : IPlugin
        {
            // Use name of the TPlugin class and fileIndex, to get a unique file for that plugin
            var directory = GetBasePluginPath<TPlugin>();
            
            // Create directory if it doesn't exist
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            // Return the full path to the file
            return Path.Combine(directory, $"{fileName}.{extension}");
        }
    }
}