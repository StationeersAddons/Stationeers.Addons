// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.Loader.Plugins
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