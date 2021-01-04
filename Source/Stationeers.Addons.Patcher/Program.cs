// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using Stationeers.Addons.Patcher.Core;

namespace Stationeers.Addons.Patcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize logger
            Logger.Init();

            // Patch the game if needed
            StandalonePatcher.Patch();
        }
    }
}