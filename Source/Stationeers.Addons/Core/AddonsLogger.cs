// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using UnityEngine;

namespace Stationeers.Addons.Core
{
    /// <summary>
    ///     Internal logging wrapper.
    /// </summary>
    internal static class AddonsLogger
    {
        /// <summary>
        ///     Logs a message to the console.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            FixMessage(ref message);
            Debug.Log($"[Stationeers.Addons] {message}");
        }

        /// <summary>
        ///     Logs a warning to the console.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warning(string message)
        {
            FixMessage(ref message);
            Debug.LogWarning($"[Stationeers.Addons - WARNING] {message}");
        }

        /// <summary>
        ///     Logs an error to the console.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            FixMessage(ref message);
            Debug.LogError($"[Stationeers.Addons - Error] {message}");
        }

        private static void FixMessage(ref string  message)
        {
            // Add 2 tabs to all new lines to the message to make it easier to read the logs.
            message = message.Replace("\n ", "\n\t");
            message = message.Replace("\n", "\n\t");
        }
    }
}