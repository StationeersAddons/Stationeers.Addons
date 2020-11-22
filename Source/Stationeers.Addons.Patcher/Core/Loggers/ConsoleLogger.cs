// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using System;

namespace Stationeers.Addons.Patcher.Core.Loggers
{
    /// <summary>
    ///     ConsoleLogger class. Implements logging loggy logs to the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            var pc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Warning] " + message);
            Console.ForegroundColor = pc;
        }

        public void LogError(string message)
        {
            var pc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[Error] " + message);
            Console.ForegroundColor = pc;
        }

        public void LogFatal(string message)
        {
            var pc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[FATAL] " + message);
            Console.ForegroundColor = pc;
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();

            Environment.Exit(-1);
        }
    }
}