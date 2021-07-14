
using System;
using System.Threading;

namespace Microsoft.Diagnostics.DebugServices
{
    /// <summary>
    /// Console output service
    /// </summary>
    public interface IContextService
    {
        /// <summary>
        /// Current context service provider. Contains the current ITarget, IThread 
        /// and IRuntime instances along with all per target and global services.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Fires anytime the current context changes.
        /// </summary>
        IServiceEvent OnContextChange { get; }
    }
}
