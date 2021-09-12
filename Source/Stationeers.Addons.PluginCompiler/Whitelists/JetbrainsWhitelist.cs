// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class JetbrainsWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.WhitelistTypesNamespaces(
                typeof(JetBrains.Annotations.PureAttribute)
            );
            whitelist.BlacklistTypes();
        }
    }
}