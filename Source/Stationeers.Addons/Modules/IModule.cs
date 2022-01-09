// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System.Collections;

namespace Stationeers.Addons.Modules
{
    /// <summary>
    ///     Basic module interface.
    /// </summary>
    internal interface IModule
    {
        /// <summary>
        ///     Initializes the module.
        ///     Similar to <see cref="Load"/> but it should not take long time to execute.
        ///     Heavier operations should be executed in the <see cref="Load"/> coroutine,
        ///     to track it's progress.
        /// </summary>
        void Initialize();
        
        /// <summary>
        ///     Coroutine that loads this module.
        ///     Called always only once.
        /// </summary>
        /// <returns>Coroutine handle.</returns>
        IEnumerator Load();

        /// <summary>
        ///     Shutdowns the module.
        /// </summary>
        void Shutdown();

        /// <summary>
        ///     The loading string that is being shown when this module is loading.
        /// </summary>
        string LoadingCaption { get; }
    }
}
