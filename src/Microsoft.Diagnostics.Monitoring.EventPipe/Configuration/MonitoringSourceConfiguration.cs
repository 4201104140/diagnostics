
using System.Collections.Generic;
using Microsoft.Diagnostics.NETCore.Client;

namespace Microsoft.Diagnostics.Monitoring.EventPipe
{
    public abstract class MonitoringSourceConfiguration
    {
        public const string MicrosoftExtensionsLoggingProviderName = "Microsoft-Extensions-Logging";

        public static IEnumerable<string> DefaultMetricProviders => new[] { MicrosoftExtensionsLoggingProviderName };

        public abstract IList<EventPipeProvider> GetProviders();

        public virtual bool RequestRundown => true;

        public virtual int BufferSizeInMB => 256;
    }
}
