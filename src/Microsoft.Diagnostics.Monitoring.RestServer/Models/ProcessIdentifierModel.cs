using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Monitoring.RestServer.Models
{
    public class ProcessIdentifierModel
    {
        public int Pid { get; set; }

        public Guid Uid { get; set; }

        //internal static ProcessIdentifierModel FromProcessInfo(IProcessInfo)
    }
}
