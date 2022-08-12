// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
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
            Debug.Log($"[Stationeers.Addons] {message}");
        }

        /// <summary>
        ///     Logs a warning to the console.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warning(string message)
        {
            Debug.LogWarning($"[Stationeers.Addons - WARNING] {message}");
        }

        /// <summary>
        ///     Logs an error to the console.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            Debug.LogError($"[Stationeers.Addons - Error] {message}");
        }
    }
}