// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    internal sealed class GameWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.BlacklistTypes(
                typeof(Assets.Scripts.Serialization.SaveLoad),
                typeof(Assets.Scripts.Serialization.XmlSaveLoad),
                typeof(Assets.Scripts.Serialization.ReplaySaveLoad),
                typeof(Assets.Scripts.Serialization.XmlSerialization)
            );
            
            // Allow all namespaces from the game assembly, but exclude some
            whitelist.WhitelistAssembly(typeof(Bread), 
                typeof(Assets.Scripts.OpenNat.Mapping),
                typeof(Open.Nat.Mapping)
            );
            
        }
    }
}