using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Diagnostics.NETCore.Client
{
    internal sealed class ReversedDiagnosticsServer : IAsyncDisposable
    {
        private static readonly TimeSpan ParseAdvertiseTimeout = TimeSpan.FromMilliseconds(250);

        private readonly CancellationTokenSource _disposalSource = new CancellationTokenSource();
        private readonly HandleableCollection<Ipc>
    }
}
