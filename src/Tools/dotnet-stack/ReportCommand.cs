using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Internal.Common.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Stack
{
    internal static class ReportCommandHandler
    {
        delegate Task<int> ReportDelegate(CancellationToken ct, IConsole console, int processId, string name, TimeSpan duration);

        private static async Task<int> Report(CancellationToken ct, IConsole console, int processId, string name, TimeSpan duration)
        {
            string tempNetTraceFileName = Path.GetRandomFileName() + ".nettrace";
            string tempEtlxFilename = "";

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (processId != 0)
                    {
                        Console.WriteLine("Can only specify either --name or --process-id option.");
                        return -1;
                    }
                    processId = CommandUtils.FindProcessIdWithName(name);
                    if (processId < 0)
                    {
                        return -1;
                    }
                }

                if (processId< 0)
                {
                    console.Error.WriteLine("Process ID should not be negative.");
                    return -1;
                }
                else if (processId == 0)
                {
                    console.Error.WriteLine("--processid is required.");
                    return -1;
                }

                var client = new DiagnosticsClient(processId);
                var providers = new List<>
            }
        }
        public static Command ReportCommand() =>
            new Command(
                name: "report",
                description: "reports the managed stacks from a running .NET process")
            {

                ProcessIdOption(),
                NameOption(),
                DurationOption()
            };

        private static Option DurationOption() =>
            new Option(
                alias: "--duration",
                description: @"When specified, will trace for the given timespan and then automatically stop the trace. Provided in the form of dd:hh:mm:ss.")
            {
                Argument = new Argument<TimeSpan>(name: "duration-timespan", getDefaultValue: () => TimeSpan.FromMilliseconds(10)),
                IsHidden = true
            };

        public static Option ProcessIdOption() =>
            new Option(
                aliases: new[] { "-p", "--process-id" },
                description: "The process id to collect the trace.")
            {
                Argument = new Argument<int>(name: "pid")
            };

        public static Option NameOption() =>
            new Option(
                aliases: new[] { "-n", "--name" },
                description: "The name of the process to collect the trace.")
            {
                Argument = new Argument<string>(name: "name")
            };
    }
}
