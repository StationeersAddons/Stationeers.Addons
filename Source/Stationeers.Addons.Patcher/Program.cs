// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System;
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

            Logger.Current.Log("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}