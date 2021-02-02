using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Diagnostics.DebugServices
{
    public enum RuntimeType
    {
        Desktop = 0,
        NetCore = 1,
        SingleFile = 2,
        Unknown = 3
    }

    /// <summary>
    /// Provides runtime info and instance
    /// </summary>
    public interface IRuntime
    {
        /// <summary>
        /// The per target services like clrmed' CirInfo abd ClrRuntime,
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// RuntimeId
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Returns the runtime OS and type
        /// </summary>
        RuntimeType RuntimeType { get; }

        /// <summary>
        /// Returns the runtime modules
        /// </summary>
        IModule RuntimeModule { get; }

        /// <summary>
        /// Return the SAC file path
        /// </summary>
        /// <returns></returns>
        string GetDacFilePath();

        /// <summary>
        /// Returns the DBI file path
        /// </summary>
        /// <returns></returns>
        string GetDbiFilePath();
    }
}
