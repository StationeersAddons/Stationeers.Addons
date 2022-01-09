// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons
{
    /// <summary>
    ///     IPlugin interface. Interface for plugins loaded by the ModLoader.
    /// </summary>
    public interface IPlugin
    {
        void OnLoad();
        void OnUnload();
    }
}