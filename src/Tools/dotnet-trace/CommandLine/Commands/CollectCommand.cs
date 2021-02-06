
using Microsoft.Tools.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
//using System.CommandLine.Rendering;
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
        delegate Task<int> CollectDelegate(CancellationToken ct, IConsole console, int processId, FileInfo output, uint buffersize, string providers, string profile, TraceFileFormat format, TimeSpan duration, string clrevents, string clreventlevel, string name, string port);

        private static async Task<int> Collect(CancellationToken ct, IConsole console, int processId, FileInfo output, uint buffersize, string providers, string profile, TraceFileFormat format, TimeSpan duration, string clrevents, string clreventlevel, string name, string diagnosticPort)
        {
            int ret = 0;
            try
            {
                Debug.Assert(output != null);
                Debug.Assert(profile != null);

                Console.WriteLine("Hi");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] {ex.ToString()}");
                ret = ErrorCodes.TracingError;
            }

            return await Task.FromResult(ret);
        }

        public static Command CollectCommand() =>
            new Command(
                name: "collect",
                description: "Collects a diagnostic trace from a currently running process")
            {
                // Handler
                //HandlerDescriptor.FromDelegate((CollectDelegate)Collect).GetCommandHandler(),
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
    }
}
