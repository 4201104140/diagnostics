
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    //internal sealed class ReverseDiagnosticsServer : IAsyncDisposable
    //{
    //    // The amount of time to allow parsing of the advertise data before cancelling. This allows the server to
    //    // remain responsive in case the advertise data is incomplete and the stream is not closed.
    //    private static readonly TimeSpan ParseAdvertiseTimeout = TimeSpan.FromMilliseconds(250);

    //    private readonly CancellationTokenSource _disponseSource = new CancellationTokenSource();

    //    private Task _listenTask;

    //    public async ValueTask DisposeAsync()
    //    {
    //        if (null != _listenTask)
    //        {
    //            try
    //            {
    //                await _listenTask.ConfigureAwait(false);
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.Fail(ex.Message);
    //            }
    //        }
    //    }
    //}
}