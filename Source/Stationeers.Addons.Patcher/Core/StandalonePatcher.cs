// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.IO;
using Stationeers.Addons.Patcher.Core.Patchers;

namespace Stationeers.Addons.Patcher.Core
{
    public static class StandalonePatcher
    {
        public static void Patch()
        {
            Logger.Current.Log("Startup");

            // Check if we are in the correct directory
            if (!File.Exists(Constants.GameExe))
                Logger.Current.LogFatal($"Could not find game executable file '{Constants.GameExe}'!");

            // Create patcher
            var patcher = new MonoPatcher();

            try
            {
                // Load and check game assembly
                patcher.Load(Constants.GameExe);

                // Check if game is patched
                // If not, patch the game.
                if (!patcher.IsPatched())
                {
                    // Patch the game
                    patcher.Patch();
                }
            }
            catch (Exception e)
            {
                Logger.Current.LogFatal(e.ToString());
            }

            // Dispose the patcher
            patcher.Dispose();
        }
    }
}
