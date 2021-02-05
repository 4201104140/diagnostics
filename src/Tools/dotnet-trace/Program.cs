using Microsoft.Internal.Common.Utils;
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
                .UseDefaults()
                .Build();
            ParseResult parseResult = parser.Parse(args);
            string parsedCommandName = parseResult.CommandResult.Command.Name;
            if (parsedCommandName == "collect")
            {
                IReadOnlyCollection<string> unparsedToken = parseResult.UnparsedTokens;
                if (unparsedToken.Count > 0)
                {
                    ProcessLauncher.Launcher.PrepareChildProcess(args);
                }
            }
            return parser.InvokeAsync(args);
        }
    }
}
