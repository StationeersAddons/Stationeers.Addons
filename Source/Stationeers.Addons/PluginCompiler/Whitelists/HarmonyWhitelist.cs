// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    internal sealed class HarmonyWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.BlacklistTypes(
                // Do not allow for transpile for now TODO: We have to figure something out
                typeof(HarmonyLib.HarmonyTranspiler),
                typeof(HarmonyLib.Transpilers),
                typeof(HarmonyLib.FileLog),
                typeof(HarmonyLib.Memory)
            );
            whitelist.WhitelistTypesNamespaces(
                typeof(HarmonyLib.Harmony) // Just allow everything
            );
        }
    }
}