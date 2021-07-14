
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Diagnostics.DebugServices.Implementation
{
    /// <summary>
    /// Manages the current target, thread and runtime contexts.
    /// </summary>
    public class ContextService : IContextService
    {
        public IServiceProvider Services => throw new NotImplementedException();

        public IServiceEvent OnContextChange => throw new NotImplementedException();
    }
}
