using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Monitoring.RestServer
{
    [Flags]
    public enum TraceProfile
    {
        Cpu = 0x1,
        Http = 0x2,
        Logs = 0x4,
        Metrics = 0x8
    }
}
