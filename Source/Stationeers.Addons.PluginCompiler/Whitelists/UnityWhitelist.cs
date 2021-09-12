// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class UnityWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes(
                typeof(UnityEngine.Debug)
            );
            whitelist.WhitelistTypesNamespaces();
            whitelist.BlacklistTypes(
                typeof(UnityEngine.Networking.UnityWebRequest)
            );
        }
    }
}