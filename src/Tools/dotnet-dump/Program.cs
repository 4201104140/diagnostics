
using Microsoft.Internal.Common.Commands;
using Microsoft.Tools.Common;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Dump
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            var parser = new CommandLineBuilder()
                .AddCommand(CollectCommand())
                .Build();

            return parser.InvokeAsync(args);
        }

        private static Command CollectCommand() =>
            new Command(name: "collect", description: "Capture dumps from a process")
            {
                // Handler
                CommandHandler.Create<IConsole, int, string, bool, Dumper.DumpTypeOption, string>(new Dumper().Collect),
                // Options

            };
    }
}
