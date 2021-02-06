using Microsoft.Internal.Common.Commands;
//using Microsoft.Internal.Common.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Trace
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            var parser = new CommandLineBuilder()
                .AddCommand(CollectCommandHandler.CollectCommand())
                .AddCommand(ProcessStatusCommandHandler.ProcessStatusCommand("Lists the dotnet processes that traces can be collected"))
                .AddCommand(ConvertCommandHandler.ConvertCommand())
                .UseDefaults()
                .Build();

            return parser.InvokeAsync(args);
        }
    }
}
