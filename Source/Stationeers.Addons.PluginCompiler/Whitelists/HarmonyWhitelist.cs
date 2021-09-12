// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class HarmonyWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.BlacklistTypes();
            whitelist.WhitelistTypesNamespaces();
        }
    }
}