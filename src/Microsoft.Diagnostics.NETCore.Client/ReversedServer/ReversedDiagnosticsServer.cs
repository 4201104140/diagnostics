using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    internal sealed class ReversedDiagnosticsServer : IAsyncDisposable
    {
        private static readonly TimeSpan ParseAdvertiseTimeout = TimeSpan.FromMilliseconds(250);

        private readonly CancellationTokenSource _disposalSource = new CancellationTokenSource();
        private readonly HandleableCollection<IpcEndpointInfo> _endpointInfos = new HandleableCollection<IpcEndpointInfo>();
        private readonly ConcurrentDictionary<Guid, HandleableCollection<Stream>> _streamCollections = new ConcurrentDictionary<Guid, HandleableCollection<Stream>>();
        private readonly string _transportPath;

        private bool _disposed = false;
        private Task _listenTask;

        public ReversedDiagnosticsServer(string transportPath) 
        {
            _transportPath = transportPath;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposalSource.Cancel();

                if (null != _listenTask)
                {
                    try
                    {
                        await _listenTask.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.Message);
                    }
                }

                _endpointInfos.Dispose();

                foreach (HandleableCollection<Stream> streamCollection in _streamCollections.Values)
                {
                    streamCollection.Dispose();
                }

                _streamCollections.Clear();

                _disposalSource.Dispose();

                _disposed = true;
            }
        }

        public void Start()
        {

        }

        
    }
}
