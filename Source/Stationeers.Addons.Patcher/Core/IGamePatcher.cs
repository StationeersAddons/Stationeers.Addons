// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;

namespace Stationeers.Addons.Patcher.Core
{
    /// <summary>
    ///     Game patcher interface, provides basic schema on how to implement a custom patcher.
    /// </summary>
    public interface IGamePatcher : IDisposable
    {
        /// <summary>
        ///     Loads game into memory.
        /// </summary>
        /// <param name="gameExe">The executable file of the game.</param>
        void Load(string gameExe);

        /// <summary>
        ///     Returns true when the game is already patched and doesn't need any modifications to run addons.
        /// </summary>
        /// <returns>True when the game is already patched.</returns>
        bool IsPatched();

        /// <summary>
        ///     Patches the game so it will run Addons Loader assembly.
        /// </summary>
        void Patch();
    }
}