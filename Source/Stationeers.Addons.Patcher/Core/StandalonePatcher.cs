// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

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

            string installInstance;

            // Check install type and current directory
            if(File.Exists(Constants.GameExe))
            {
                // Found game install
                installInstance = Constants.GameExe;
            }
            else if (File.Exists(Constants.ServerExe))
            {
                // Found server install
                installInstance = Constants.ServerExe;
            }
            else
            {
                // No install found
                installInstance = null;
                Logger.Current.LogFatal($"Could not find executable file '{Constants.GameExe}' or {Constants.ServerResourcesDir}!");
            }

            // Create patcher
            var patcher = new MonoPatcher();

            try
            {
                // Load and check game assembly
                patcher.Load(installInstance);

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
