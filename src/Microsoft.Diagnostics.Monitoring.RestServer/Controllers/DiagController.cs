using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Monitoring.RestServer.Controllers
{
    [Route("")] // Root
    [ApiController]
    public class DiagController : ControllerBase
    {
        private const string ArtifactType_Dump = "dump";
        private const string ArtifactType_GCDump = "gcdump";
        private const string ArtifactType_Logs = "logs";
        private const string ArtifactType_Trace = "trace";

        private const TraceProfile DefaultTraceProfiles = TraceProfile.Cpu | TraceProfile.Http | TraceProfile.Metrics;
        private static readonly MediaTypeHeaderValue NdJsonHeader = new MediaTypeHeaderValue(ContentTypes.ApplicationNdJson);
        private static readonly MediaTypeHeaderValue EventStreamHeader = new MediaTypeHeaderValue(ContentTypes.TextEventStream);

        private readonly ILogger<DiagController> _logger;

        public DiagController(ILogger<DiagController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;

        }

        [HttpGet("processes")]
        public Task<ActionResult<IEnumerable<>>>
        {
            return new List<int> { 1, 2, 3 };
        }
    }
}
