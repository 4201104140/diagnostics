// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Internal.Common.Utils;
using Microsoft.Tools.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Rendering;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Trace
{
    internal static class CollectCommandHandler
    {
        delegate Task<int> CollectDelegate(CancellationToken ct, IConsole console, int processId, FileInfo output, uint buffersize, string providers, string profile, bool showchildio);

        /// <summary>
        /// Collects a diagnostic trace from a currently running process or launch a child process and trace it.
        /// Append -- to the collect command to instruct the tool to run a command and trace it immediately. By default the IO from this process is hidden, but the --show-child-io option may be used to show the child process IO.
        /// </summary>
        /// <param name="ct">The cancellation token</param>
        /// <param name="console"></param>
        /// <param name="processId">The process to collect the trace from.</param>
        /// <param name="output">The output path for the collected trace data.</param>
        /// <param name="buffersize">Sets the size of the in-memory circular buffer in megabytes.</param>
        /// <param name="providers">A list of CLR events to be taken.</param>
        /// <param name="profile">A named pre-defined set of CLR events</param>
        /// <param name="showchildio">Should IO from a child process be hidden.</param>
        /// <returns></returns>
        private static async Task<int> Collect(CancellationToken ct, IConsole console, int processId, FileInfo output, uint buffersize, string providers, string profile, bool showchildio)
        {
            bool collectionStopped = false;
            bool cancelOnEnter = true;
            bool cancelOnCtrlC = true;
            bool printStatusOverTime = true;
            int ret = ReturnCode.Ok;

            try
            {
                Debug.Assert(output != null);
                Debug.Assert(profile != null);

                if (ProcessLauncher.Launcher.HasChildProc && showchildio)
                {
                    // If showing IO, then all IO (including CtrlC) behavior is delegated to the child process
                    cancelOnCtrlC = false;
                    cancelOnEnter = false;
                    printStatusOverTime = false;
                }
                else
                {
                    cancelOnCtrlC = true;
                    cancelOnEnter = !Console.IsInputRedirected;
                    printStatusOverTime = !Console.IsOutputRedirected;
                }

                if (!cancelOnCtrlC)
                {
                    ct = CancellationToken.None;
                }

                if (!ProcessLauncher.Launcher.HasChildProc)
                {
                    if (showchildio)
                    {
                        Console.WriteLine("--show-child-io must not be specified when attaching to a process");
                        return ReturnCode.ArgumentError;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] {ex.ToString()}");
                collectionStopped = true;
                ret = ReturnCode.TracingError;
            }
            finally
            {
                if (printStatusOverTime)
                {
                }

                if (ProcessLauncher.Launcher.HasChildProc)
                {
                    if (!collectionStopped || ct.IsCancellationRequested)
                    {
                        ret = ReturnCode.TracingError;
                    }

                    // If we launched a child proc that hasn't exited yet, terminate it before we exit.
                    if (!ProcessLauncher.Launcher.ChildProc.HasExited)
                    {
                        ProcessLauncher.Launcher.ChildProc.Kill();
                    }
                }
            }
            return await Task.FromResult(ret);
        }

        public static Command CollectCommand() =>
            new Command(
                name: "collect",
                description: "Collects a diagnostic trace from a currently running process or launch a child process and trace it. Append -- to the collect command to instruct the tool to run a command and trace it immediately. When tracing a child process, the exit code of dotnet-trace shall be that of the traced process unless the trace process encounters an error.")
            {
                // Handler
                HandlerDescriptor.FromDelegate((CollectDelegate)Collect).GetCommandHandler(),
                // Options
                CommonOptions.ProcessIdOption(),
            };
    }
}
