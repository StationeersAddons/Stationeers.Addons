// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    internal sealed class JetbrainsWhitelist : IWhitelistRegistry
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