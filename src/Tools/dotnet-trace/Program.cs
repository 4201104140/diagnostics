﻿
using Microsoft.Internal.Common.Commands;
using Microsoft.Internal.Common.Utils;
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
                .AddCommand(ListProfilesCommandHandler.ListProfilesCommand())
                .AddCommand(ConvertCommandHandler.ConvertCommand())
                .UseDefaults()
                .Build();
            ParseResult parseResult = parser.Parse(args);
            string parsedCommandName = parseResult.CommandResult.Command.Name;
            if (parsedCommandName == "collect")
            {
                IReadOnlyCollection<string> unparsedTokens = parseResult.UnparsedTokens;
                // If we notice there are unparsed tokens, user might want to attach on startup.
                if (unparsedTokens.Count > 0)
                {
                    ProcessLauncher.Launcher.PrepareChildProcess(args);
                }
            }
            return parser.InvokeAsync(args);
        }
    }
}