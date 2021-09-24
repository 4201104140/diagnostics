using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Trace
{
    internal sealed class ListProfilesCommandHandler
    {
        public static async Task<int> GetProfiles(IConsole console)
        {
            try
            {
                foreach (var profile in )
            }
        }

        public static Command ListProfilesCommand() =>
            new Command(
                name: "list-profiles",
                description: "Lists pre-built tracing profile with a description of what provider and filter are in each profile")
            {
                Handler = CommandHandler.Create<IConsole>(GetProfiles),
            };

        internal static IEnumerable<Profil>
    }
}
