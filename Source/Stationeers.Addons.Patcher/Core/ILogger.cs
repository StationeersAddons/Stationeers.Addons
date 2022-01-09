// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.Patcher.Core
{
    /// <summary>
    ///     ILogger interface, provides basic logging schema.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Logs simple message.
        /// </summary>
        /// <param name="message">The log message.</param>
        void Log(string message);

        /// <summary>
        ///     Logs warning message.
        /// </summary>
        /// <param name="message">The log message.</param>
        void LogWarning(string message);

        /// <summary>
        ///     Logs error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        void LogError(string message);

        /// <summary>
        ///     Logs fatal message and waits for the user input, then our application suicides when it doesn't like the user.
        /// </summary>
        /// <param name="message">The log message.</param>
        void LogFatal(string message);
    }
}