// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.Common.Utils
{
    // <summary>
    // ProcessLauncher is a child-process launcher for "diagnostics tools at startup" scenarios
    // It launches the target process at startup and passes its processId to the corresponding Command handler.
    // </summary>
    internal class ProcessLauncher
    {
        private Process _childProc = null;
        private Task _stdOutTask = Task.CompletedTask;
        private Task _stdErrTask = Task.CompletedTask;
        internal static ProcessLauncher Launcher = new ProcessLauncher();

        public void PrepareChildProcess(string[] args)
        {
            int unparsedTokenIdx = FindUnparsedTokenIndex(args);
            if (unparsedTokenIdx < 0)
            {
                return;
            }

            _childProc = new Process();
            _childProc.StartInfo.FileName = args[unparsedTokenIdx];
            string arguments = "";
            for (int i = unparsedTokenIdx + 1; i < args.Length; i++)
            {
                if (args[i].Contains(" "))
                {
                    arguments += $"\"{args[i].Replace("\"", "\\\"")}\"";
                }
                else
                {
                    arguments += args[i];
                }

                if (i != args.Length)
                    arguments += " ";
            }
            _childProc.StartInfo.Arguments = arguments;
        }

        private int FindUnparsedTokenIndex(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--" && i < (args.Length - 1)) return i + 1;
            }
            return -1;
        }

        private async Task ReadAndIgnoreAllStreamAsync(StreamReader streamToIgnore, CancellationToken cancelToken)
        {
            Memory<char> memory = new char[4096];
            while (await streamToIgnore.ReadAsync(memory, cancelToken) != 0)
            {
            }
        }

        public bool HasChildProc
        {
            get
            {
                return _childProc != null;
            }
        }

        public Process ChildProc
        {
            get
            {
                return _childProc;
            }
        }
        public bool Start(string diagnosticTransportName, CancellationToken ct, bool showChildIO, bool printLaunchCommand)
        {
            _childProc.StartInfo.UseShellExecute = false;
            _childProc.StartInfo.RedirectStandardOutput = !showChildIO;
            _childProc.StartInfo.RedirectStandardError = !showChildIO;
            _childProc.StartInfo.RedirectStandardInput = !showChildIO;
            _childProc.StartInfo.Environment.Add("DOTNET_DiagnosticPorts", $"{diagnosticTransportName}");
            try
            {
                if (printLaunchCommand)
                {
                    Console.WriteLine($"Launching: {_childProc.StartInfo.FileName} {_childProc.StartInfo.Arguments}");
                }
                _childProc.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot start target process: {_childProc.StartInfo.FileName} {_childProc.StartInfo.Arguments}");
                Console.WriteLine(e.ToString());
                return false;
            }
            if (!showChildIO)
            {
                _stdOutTask = ReadAndIgnoreAllStreamAsync(_childProc.StandardOutput, ct);
                _stdErrTask = ReadAndIgnoreAllStreamAsync(_childProc.StandardError, ct);
            }

            return true;
        }

        public void Cleanup()
        {
            if (_childProc != null && !_childProc.HasExited)
            {
                try
                {
                    _childProc.Kill();
                }
                // if process exited while we were trying to kill it, it can throw IOE 
                catch (InvalidOperationException) { }
                _stdOutTask.Wait();
                _stdErrTask.Wait();
            }
        }
    }

    internal class DiagnosticsClientHolder : IDisposable
    {
        public DiagnosticsClient Client;
        public IpcEndpointInfo EndpointInfo;

        private readonly string _port;

        public DiagnosticsClientHolder(DiagnosticsClient client)
        {
            Client = client;
            _port = null;
        }

        public async void Dispose()
        {
            if (!string.IsNullOrEmpty(_port) && File.Exists(_port))
            {
                File.Delete(_port);
            }
            ProcessLauncher.Launcher.Cleanup();
            await Task.CompletedTask;
        }
    }

    internal class DiagnosticsClientBuilder
    {
        private string _toolName;
        private int _timeoutInSec;

        private string GetTransportName(string toolName) => $"{toolName}-{Process.GetCurrentProcess().Id}-{DateTime.Now:yyyyMMdd_HHmmss}.socket";

        public DiagnosticsClientBuilder(string toolName, int timeoutInSec)
        {
            _toolName = toolName;
            _timeoutInSec = timeoutInSec;
        }

        public async Task<DiagnosticsClientHolder> Build(CancellationToken ct, int processId, string portName, bool showChildIO, bool printLaunchCommand)
        {
            if (ProcessLauncher.Launcher.HasChildProc)
            {
                // Create and start the reversed server
                await Task.CompletedTask;
            }

            return new DiagnosticsClientHolder(new DiagnosticsClient(processId));
        }
    }
}