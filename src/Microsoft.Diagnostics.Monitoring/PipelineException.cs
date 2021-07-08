
using System;

namespace Microsoft.Diagnostics.Monitoring
{
    internal class PipelineException : MonitoringException
    {
        public PipelineException(string message) : base(message) { }
        public PipelineException(string message, Exception inner) : base(message, inner) { }
    }
}