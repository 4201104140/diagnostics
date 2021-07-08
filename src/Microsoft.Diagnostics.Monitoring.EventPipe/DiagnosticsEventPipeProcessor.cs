
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Monitoring.EventPipe
{
    internal partial class DiagnosticsEventPipeProcessor : IAsyncDisposable
    {
        private readonly MonitoringSourceConfiguration _configuration;
        private readonly Func<EventPipeEventSource, Func<Task>, CancellationToken, Task> _onEventSourceAvailable;

        private readonly object _lock = new object();

        private TaskCompletionSource<bool> _initialized;
        private TaskCompletionSource<bool> _sessionStarted;
        private EventPipeEventSource _eventSource;
        private Func<Task> _stopFunc;
        private bool _disposed;

        // Allows tests to know when the event pipe session has started so that the
        // target application can start producing events.
        internal Task SessionStarted => _sessionStarted.Task;

        public async ValueTask DisposeAsync()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;
            }

            _initialized.TrySetCanceled();
            try
            {
                await _initialized.Task;
            }
            catch
            {
            }

            _sessionStarted.TrySetCanceled();

            _eventSource?.Dispose();
        }
    }
}
