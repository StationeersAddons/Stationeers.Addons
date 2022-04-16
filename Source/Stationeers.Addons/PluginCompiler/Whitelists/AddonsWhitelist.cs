// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using Stationeers.Addons.API;

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class AddonsWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes(
                typeof(IPlugin),
                typeof(Globals)
            );

            whitelist.WhitelistTypesNamespaces(
                typeof(BundleManager)
            );
            
            whitelist.BlacklistTypes();
        }
    }
}