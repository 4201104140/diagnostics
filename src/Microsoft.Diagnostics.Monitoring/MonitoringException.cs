using System;

namespace Microsoft.Diagnostics.Monitoring
{
    internal class MonitoringException : Exception
    {
        public MonitoringException(string message) : base(message) { }

        public MonitoringException(string message, Exception innerException) : base(message, innerException) { }
    }
}