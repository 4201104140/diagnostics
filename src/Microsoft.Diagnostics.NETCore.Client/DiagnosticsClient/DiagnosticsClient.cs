using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Diagnostics.NETCore.Client
{
    /// <summary>
    /// This is a top-level class that contains methods to send various diagnostics command to the runtime.
    /// </summary>
    public sealed class DiagnosticsClient
    {
        private readonly IpcEndpoint _endpoint;

        public DiagnosticsClient(int processId)
        {

        }

        internal DiagnosticsClient(IpcEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public static IEnumerable<int> GetPublishedProcess()
        {
            static IEnumerable<int> GetAllPublishedProcess()
            {
                foreach (var port in Directory.GetFiles(PidIpcEndpoint))
            }
        }
    }
}
