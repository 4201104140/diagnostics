
using Microsoft.Diagnostics.NETCore.Client;
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
        delegate Task<int> CollectDelegate(CancellationToken ct, IConsole console, int processId, FileInfo output, uint buffersize, string providers, string profile, TraceFileFormat format, TimeSpan duration, string clrevents, string clreventlevel, string name, string port, bool showchildio, bool resumeRuntime);

        private static async Task<int> Collect(CancellationToken ct, IConsole console, int processId, FileInfo output, uint buffersize, string providers, string profile, TraceFileFormat format, TimeSpan duration, string clrevents, string clreventlevel, string name, string diagnosticPort, bool showchildio, bool resumeRuntime)
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
                    printStatusOverTime = !Console.IsInputRedirected;
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
                    if (CommandUtils.ValidateArgumentsForAttach(processId, name, diagnosticPort, out int resolvedProcessId))
                    {
                        processId = resolvedProcessId;
                    }
                    else
                    {
                        return ReturnCode.ArgumentError;
                    }
                }
                else if (!CommandUtils.ValidateArgumentsForChildProcess(processId, name, diagnosticPort))
                {
                    return ReturnCode.ArgumentError;
                }

                if (profile.Length == 0 && providers.Length == 0 && clrevents.Length == 0)
                {
                    Console.Out.WriteLine("No profile or providers specified, defaulting to trace profile 'cpu-sampling'");
                    profile = "cpu-sampling";
                }

                Dictionary<string, string> enabledBy = new Dictionary<string, string>();

                var providerCollection = Extensions.ToProviders(providers);
                foreach (EventPipeProvider providerCollectionProvider in providerCollection)
                {
                    enabledBy[providerCollectionProvider.Name] = "--providers";
                }

                if (profile.Length != 0)
                {
                    var selectedProfile = ListProfilesCommandHandler.DotNETRuntimeProfiles
                        .FirstOrDefault(p => p.Name.Equals(profile, StringComparison.OrdinalIgnoreCase));
                    if (selectedProfile == null)
                    {
                        Console.Error.WriteLine($"Invalid profile name: {profile}");
                        return ReturnCode.ArgumentError;
                    }

                    Profile.MergeProfileAndProviders(selectedProfile, providerCollection, enabledBy);
                }


            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] {ex.ToString()}");
                //collectionStopped = true;
                ret = ReturnCode.TracingError;
            }
            finally
            {
                if (printStatusOverTime)
                {
                    if (console.GetTerminal() != null)
                        Console.CursorVisible = true;
                }

                if (ProcessLauncher.Launcher.HasChildProc)
                {
                    if (!collectionStopped || ct.IsCancellationRequested)
                    {
                        ret = ReturnCode.TracingError;
                    }

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
                description: "Collects a diagnostic trace from a currently running process")
            {
                // Handler
                HandlerDescriptor.FromDelegate((CollectDelegate)Collect).GetCommandHandler(),
                // Options
                CommonOptions.ProcessIdOption(),

                CommonOptions.FormatOption(),

                CommonOptions.NameOption()
            };

        private static uint DefaultCircularBufferSizeInMB() => 256;

        private static Option CircularBufferOption() =>
            new Option(
                alias: "--buffersize",
                description: $"Sets the size of the in-memory circular buffer in megabytes. Default {DefaultCircularBufferSizeInMB()} MB.")
            {

            };

        private static string GetSize(long length)
        {
            if (length > 1e9)
                return String.Format("{0,-8} (GB)", $"{length / 1e9:0.00##}");
            else if (length > 1e6)
                return String.Format("{0,-8} (MB)", $"{length / 1e6:0.00##}");
            else if (length > 1e3)
                return String.Format("{0,-8} (KB)", $"{length / 1e3:0.00##}");
            else
                return String.Format("{0,-8} (B)", $"{length / 1.0:0.00##}");
        }

        public static string DefaultTraceName => "trace.nettrace";
    }
}
