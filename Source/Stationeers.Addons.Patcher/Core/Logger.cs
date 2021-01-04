// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

using Stationeers.Addons.Patcher.Core.Loggers;

namespace Stationeers.Addons.Patcher.Core
{
    public static class Logger
    {
        public static ILogger Current { get; private set; }

        public static void Init()
        {
            Current = new ConsoleLogger();
        }
    }
}