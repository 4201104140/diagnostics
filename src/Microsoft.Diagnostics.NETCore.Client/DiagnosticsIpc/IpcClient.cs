// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    internal class IpcClient
    {
        // The amount of time to wait for a stream to be available for consumption by the Connect method.
        // Normally expect the runtime to respond quickly but resource constrained machines may take longer.
        internal static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Sends a single DiagnosticsIpc Message to the dotnet process with PID processId.
        /// </summary>
        /// <param name="endpoint">An endpoint that provides a diagnostics connection to a runtime instance.</param>
        /// <param name="message">The DiagnosticsIpc Message to be sent</param>
        /// <returns>The response DiagnosticsIpc Message from the dotnet process</returns>
        public static IpcMessage SendMessage(IpcEndpoint endpoint, IpcMessage message)
        {
            using IpcResponse response = new
        }

        public static IpcResponse SendMessageGetContinuation(IpcEndpoint endpoint, IpcMessage message)
        {
            Stream stream = null;
            try
            {
                stream = endpoint.Connect(ConnectTimeout);

                Write(stream, message);

                IpcMessage response = Read
            }
        }

        private static void Write(Stream stream, IpcMessage message)
        {
            byte[] buffer = message.Serialize();
            stream.Write(buffer, 0, buffer.Length);
        }

        private static IpcMessage Read(Stream stream)
        {
            return IpcMessage.Parse(stream);
        }
    }
}
