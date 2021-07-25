// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    /// <summary>
    /// This is a top-level class that contains methods to send various diagnostics command to the runtime.
    /// </summary>
    public sealed class DiagnosticsClient
    {
        private readonly IpcEndpoint _endpoint;

        public DiagnosticsClient(int processId) :
            this(new PidIpcEndpoint(processId))
        {
        }

        internal DiagnosticsClient(IpcEndpointConfig config) :
            this(new DiagnosticPortIpcEndpoint(config))
        {
        }

        internal DiagnosticsClient(IpcEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        /// <summary>
        /// Trigger a core dump generation.
        /// </summary> 
        /// <param name="dumpType">Type of the dump to be generated</param>
        /// <param name="dumpPath">Full path to the dump to be generated. By default it is /tmp/coredump.{pid}</param>
        /// <param name="logDumpGeneration">When set to true, display the dump generation debug log to the console.</param>
        public void WriteDump(DumpType dumpType, string dumpPath, bool logDumpGeneration = false)
        {
            if (string.IsNullOrEmpty(dumpPath))
                throw new ArgumentNullException($"{nameof(dumpPath)} required");

            
        }

        /// <summary>
        /// Get all the active processes that can be attached to.
        /// </summary>
        /// <returns>
        /// IEnumerable of all the active process IDs.
        /// </returns>
        public static IEnumerable<int> GetPublishedProcesses()
        {
            static IEnumerable<int> GetAllPublishedProcesses()
            {
                foreach (var port in Directory.GetFiles(PidIpcEndpoint.IpcRootPath))
                {
                    var fileName = new FileInfo(port).Name;
                    var match = Regex.Match(fileName, PidIpcEndpoint.DiagnosticsPortPattern);
                    if (!match.Success) continue;
                    var group = match.Groups[1].Value;
                    if (!int.TryParse(group, NumberStyles.Integer, CultureInfo.InvariantCulture, out var processId))
                        continue;

                    yield return processId;
                }
            }

            return GetAllPublishedProcesses().Distinct();
        }

        internal ProcessInfo GetProcessInfo()
        {
            // RE: https://github.com/dotnet/runtime/issues/54083
            // If the GetProcessInfo2 command is sent too early, it will crash the runtime instance.
            // Disable the usage of the command until that issue is fixed.

            // Attempt to get ProcessInfo v2
            //ProcessInfo processInfo = GetProcessInfo2();
            //if (null != processInfo)
            //{
            //    return processInfo;
            //}

            // Attempt to get ProcessInfo v1
            IpcMessage message = new IpcMessage(DiagnosticsServerCommandSet.Process, (byte)ProcessCommandId.GetProcessInfo);
            var response = IpcClient.SendMessage(_endpoint, message);
            switch ((DiagnosticsServerResponseId)response.Header.CommandId)
            {
                case DiagnosticsServerResponseId.Error:
                    var hr = BitConverter.ToInt32(response.Payload, 0);
                    throw new ServerErrorException($"Get process info failed (HRESULT: 0x{hr:X8})");
                case DiagnosticsServerResponseId.OK:
                    return ProcessInfo.ParseV1(response.Payload);
                default:
                    throw new ServerErrorException($"Get process info failed - server responded with unknown command");
            }
        }
    }
}