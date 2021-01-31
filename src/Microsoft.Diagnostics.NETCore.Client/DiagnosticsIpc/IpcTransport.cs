using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    internal abstract class IpcTransport
    {
        public abstract Stream Connect(TimeSpan timeout);

        public abstract Task<Stream> ConnectAsync(CancellationToken token);

        public abstract void WaitForConnection(TimeSpan timeout);

        public abstract Task WaitForConnectionAsync(CancellationToken token);
    }

    internal class ServerIpcEndpoint : IpcTransport
    {
        private readonly Guid _runtimeId;
        private readonly 
    }
}
