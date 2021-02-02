using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using System.Threading;
using System.IO;

namespace Microsoft.Diagnostics.NETCore.Client
{
    public class TestRunner : IDisposable
    {
        private Process testProcess;
        private ProcessStartInfo startInfo;
        private ITestOutputHelper outputHelper;
        private CancellationTokenSource cts;

        public TestRunner(string testExePath, ITestOutputHelper _outputHelper = null,
            bool redirectError = false, bool redirectInput = false)
        {
            startInfo = new ProcessStartInfo(CommonHelper.HostExe, testExePath);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = redirectError;
            startInfo.RedirectStandardInput = redirectInput;
            outputHelper = _outputHelper;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                // Make a good will attempt to end the tracee process
                // and its process tree
                testProcess?.Kill(entireProcessTree: true);
            }
            catch { }

            if (disposing)
            {
                testProcess?.Dispose();
            }

            cts.Dispose();
        }

        public void AddEnvVar(string key, string value)
        {
            startInfo.EnvironmentVariables[key] = value;
        }

        public StreamWriter StandardInput => testProcess.StandardInput;
        public StreamReader StandardOutput => testProcess.StandardOutput;
        public StreamReader StandardError => testProcess.StandardError;

        public void Start(int timeoutInMSPipeCreation = 15_000, int testProcessTimeout = 30_000)
        {
            if (outputHelper != null)
                outputHelper.WriteLine($"[{DateTime.Now.ToString()}] Launching test: " + startInfo.FileName + " " + startInfo.Arguments);

            testProcess = new Process();
            testProcess.StartInfo = startInfo;
            testProcess.EnableRaisingEvents = true;

            if (!testProcess.Start())
            {
                outputHelper.WriteLine($"Could not start process: " + startInfo.FileName);
            }

            if (testProcess.HasExited)
            {
                outputHelper.WriteLine($"Process " + startInfo.FileName + " came back as exited");
            }

            cts = new CancellationTokenSource(testProcessTimeout);
            cts.Token.Register(() => testProcess.Kill());

            if (outputHelper != null)
            {
                outputHelper.WriteLine("");
            }
        }
    }
}
