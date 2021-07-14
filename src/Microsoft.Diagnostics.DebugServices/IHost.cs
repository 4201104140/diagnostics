using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Diagnostics.DebugServices
{

    /// <summary>
    /// Host interface
    /// </summary>
    public interface IHost
    {

        /// <summary>
        /// Global service provider
        /// </summary>
        IServiceProvider Services { get; }
    }


}
