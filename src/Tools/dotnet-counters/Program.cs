
using Microsoft.Internal.Common.Commands;
using Microsoft.Internal.Common.Utils;
using Microsoft.Tools.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Counters
{
    public enum CountersExportFormat { csv, json };

    internal class Program
    {
        delegate Task<int> CollectDelegate(
            CancellationToken ct,
            List<string> counter_list,
            string counters,
            IConsole console,
            int processId,
            int refreshInterval,
            CountersExportFormat format,
            string output,
            string processName,
            string port,
            bool resumeRuntime,
            int maxHistograms,
            int maxTimeSeries);
        delegate Task<int> MonitorDelegate(
            CancellationToken ct,
            List<string> counter_list,
            string counters,
            IConsole console,
            int processId,
            int refreshInterval,
            string processName,
            string port,
            bool resumeRuntime,
            int maxHistograms,
            int maxTimeSeries);

        private static Command MonitorCommand() =>
            new Command(
                name: "monitor",
                description: "Start monitoring a .NET application")
            {
                // Handler
                HandlerDescriptor.FromDelegate((MonitorDelegate)new CounterMonitor().Monitor).GetCommandHandler(),
            };

        private static Command CollectCommand() =>
            new Command(
                name: "collect",
                description: "Monitor counters in a .NET application and export the result into a file")
            {
                HandlerDescriptor.FromDelegate((CollectDelegate)new CounterMonitor().Collect).GetCommandHandler()
            };


        private static Task<int> Main(string[] args)
        {
            var parser = new CommandLineBuilder()
                .AddCommand(MonitorCommand())
                .AddCommand(CollectCommand())
                .Build();

            ParseResult parseResult = parser.Parse(args);
            string parsedCommandName = parseResult.CommandResult.Command.Name;
            if (parsedCommandName == "monitor" || parsedCommandName == "collect")
            {
                IReadOnlyCollection<string> unparsedTokens = parseResult.UnparsedTokens;

                if (unparsedTokens.Count > 0)
                {

                }
            }

            return parser.InvokeAsync(args);
        }
    }
}