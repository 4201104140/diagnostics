using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Counters
{
    public class CounterProvider
    {
        public string Name { get; }
        public string Description { get; }
        public string Keywords { get; }
        public string Level { get; }
        public Dictionary<string, CounterProfile>
    }

    public class CounterProfile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] SupportedVersions { get; set; }
    }
}
