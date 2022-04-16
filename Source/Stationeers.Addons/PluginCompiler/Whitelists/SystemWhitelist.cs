// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class SystemWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes(
                typeof(object),
                typeof(string),
                typeof(bool),
                typeof(char),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal)
                
                // TODO: Add more
            );
            whitelist.BlacklistTypes();

            whitelist.WhitelistTypesNamespaces(
                typeof(System.Collections.IEnumerator),
                typeof(System.Collections.Generic.List<>),
                typeof(System.Collections.Concurrent.Partitioner<>),
                typeof(System.Linq.Enumerable),
                typeof(System.Text.StringBuilder)
            );
        }
    }
}