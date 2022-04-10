// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Stationeers.Addons.Modules.Plugins
{
    internal class PluginCompiler : IDisposable
    {
        private static bool _hasErrors;
        private ProcessStartInfo _compilerStartInfo;

        public PluginCompiler()
        {
            // TODO: Host plugin compiler process using mono-runtime on Linux
            
            _compilerStartInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(Environment.CurrentDirectory, "AddonManager", "Stationeers.Addons.PluginCompiler.exe"),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "AddonManager")
            };
        }

        public void Dispose()
        {
            _compilerStartInfo = null;
        }

        public void CompileScripts(string addonName, string addonDirectory, string[] addonScripts, out bool isSuccess)
        {
            var scriptFiles = addonScripts.Select(addonScript => addonScript.Replace(addonDirectory, "")).ToList();

            _compilerStartInfo.Environment.Add("AddonName", addonName);
            _compilerStartInfo.Environment.Add("AddonDirectory", addonDirectory);
            _compilerStartInfo.Environment.Add("AddonScripts", string.Join(";", scriptFiles));

            _hasErrors = false;
            
            // Start compiler
            var process = Process.Start(_compilerStartInfo);
            process.ErrorDataReceived += OnCompilerError;
            process.OutputDataReceived += OnCompilerLog;
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            // TODO: Better exit handling that will not hang the game (wait inside coroutine)
            process.WaitForExit();
            process.CancelOutputRead();
            process.CancelErrorRead();
            process.Dispose();

            isSuccess = !_hasErrors;
            _hasErrors = false;
        }

        private static void OnCompilerLog(object sender, DataReceivedEventArgs e)
        {
            Debug.unityLogger.logHandler.LogFormat(LogType.Log, null, e.Data);
        }

        private static void OnCompilerError(object sender, DataReceivedEventArgs e)
        {
            // Not sure what is going on, but we're receiving an error from the process, but it is... empty!?
            // Checked it and the plugins compile normally. So just make sure that this fluke does not fail our compilation.
            if(string.IsNullOrEmpty(e.Data)) return;
            
            _hasErrors = true;
            Debug.unityLogger.logHandler.LogFormat(LogType.Error, null, e.Data);
        }
    }
}
