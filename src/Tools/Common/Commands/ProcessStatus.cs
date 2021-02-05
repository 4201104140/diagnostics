// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Process = System.Diagnostics.Process;

namespace Microsoft.Internal.Common.Commands
{
    public class ProcessStatusCommandHandler
    {
        public static Command ProcessStatusCommand(string description) =>
            new Command(name: "ps", description)
            {
                Handler = CommandHandler.Create<IConsole>()
            };

        public static void PrintProcessStatus(IConsole console)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                var processes = DiagnosticsClient.GetPublishedProcesses
            }
        }
    }
}
