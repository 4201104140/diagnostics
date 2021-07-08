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
    [HostRestriction]
    public class DiagController : ControllerBase
    {
        private const string ArtifactType_Dump = "dump";
        private const string ArtifaceType_GCDump = "gcdump";
        private const string ArtifaceType_Logs = "logs";
        private const string ArtifaceType_Trace = "trace";

        private readonly ILogger<DiagController> _logger;

        public DiagController(ILogger<DiagController> logger)
        {
            _logger = logger;
        }

        public Task<ActionResult<IEnumerable>>
    }
}
