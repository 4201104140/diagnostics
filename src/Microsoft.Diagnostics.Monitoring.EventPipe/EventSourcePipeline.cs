
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Monitoring.EventPipe
{
    internal abstract class EventSourcePipeline<T> : Pipeline, IEventSourcePipelineInternal where T : EventSourcePipelineSettings
    {
        private readonly Lazy<DiagnosticsEventPipeProcessor> _processor;
        public DiagnosticsClient Client { get; }
        public T Settings { get; }

        Task IEventSourcePipelineInternal.SessionStarted => _processor.Value.SessionStarted;

        
    }

    internal interface IEventSourcePipelineInternal
    {
        // Allows tests to know when the event pipe session has started so that the
        // target application can start producing events.
        Task SessionStarted { get; }
    }
}
