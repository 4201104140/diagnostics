
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Monitoring
{
    /// <summary>
    /// A pipeline controls data which is flowing from some source to sink asynchronously.
    /// This class allows the flow to be started and stopped. The concrete class
    /// determines what data is being collected and where it will flow to.
    ///
    /// The pipeline is logically in one of these states:
    /// Unstarted - After the object is constructed and prior to calling RunAsync or
    /// StopAsync. No data is flowing.
    /// Running - The pipeline is doing whatever asynchronous work is necessary to flow
    /// data. Unstarted transitions to Running with a call to RunAsync()
    /// Stopping - The pipeline is doing a graceful shutdown to stop receiving any new
    /// data and drain any in-flight data to the sink. Unstarted or Running transitions to
    /// Stopping with a call to StopAsync(). Pipelines may also automatically enter a Stopping
    /// state when there is no data left to receive from the source.
    /// Stopped - All asynchronous work has ceased and the pipeline can not be restarted. This
    /// transition happens asynchronously from the stopping state when there is no
    /// work left to be done. The only way to be certain you have reached this state is to
    /// observe that the Task returned by StopAsync() or RunAsync() is completed or cancelled,
    /// usually by awaiting it.
    /// Aborting - Entered by cancelling any of the tokens to StopAsync or RunAsync, or by explicitly
    /// calling DisposeAsync.
    /// Unstarted -> Running -> Stopping -> Stopped
    ///           |           |               ^
    ///           |           V               |
    ///           -------> Aborting ----------|
    /// </summary>
    internal abstract class Pipeline : IAsyncDisposable
    {
        private readonly CancellationTokenSource _disposeSource = new CancellationTokenSource();
        private object _lock = new object();
        private bool _isCleanedUp;
        private Task _runTask;
        private Task _stopTask;
        private Task _cleanupTask;

        public Task StopAsync(CancellationToken token)
        {
            Task stopTask = null;
            lock (_lock)
            {
                if (_isCleanedUp)
                {
                    stopTask = _stopTask ?? Task.CompletedTask;
                }
                else if (_runTask == null)
                {
                    throw new PipelineException("Unable to stop unstarted pipeline");
                }
                else
                {
                    if (_stopTask == null)
                    {
                        
                    }
                    stopTask = _stopTask;
                }
                return stopTask;
            }
        }

        /// <summary>
        /// Requests that the pipeline abort the data flow as quickly as possible and transitions
        /// to Stopped state. Note that this will not cause the pipeline to trigger ObjectDisposedException.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            lock (_lock)
            {
                if (_isCleanedUp)
                {
                    return;
                }
                _isCleanedUp = true;
            }
            _disposeSource.Cancel();

            //It's necessary to fully acquire the task, await it, and then move on to the next task.
            await SafeExecuteTask(() => _runTask);
            await SafeExecuteTask(() => _stopTask);
            await SafeExecuteTask(() => _cleanupTask);

            _disposeSource.Dispose();
        }

        private async Task SafeExecuteTask(Func<Task> acquireTask)
        {
            Task task = null;
            lock (_lock)
            {
                task = acquireTask();
            }

            if (task != null)
            {
                try
                {
                    await task;
                }
                catch
                {
                }
            }
        }
    }
}
