using System;
using System.Collections.Generic;
using System.Text;

namespace SOS.Hosting
{
    public sealed class SOSHost : IDisposable
    {
        // This is what dbgeng/IDebuggerServices returns for non-PE modules that don't have a timestamp
        internal const uint InvalidTimeStamp = 0xFFFFFFFE;
        internal const uint InvalidChecksum = 0xFFFFFFFF;

        internal readonly IServiceProvider Services;
        
        void IDisposable.Dispose()
        {

        }
    }
}
