// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    public class EventPipeSession : IDisposable
    {
        private long _sessionId;
        private IpcEndpoint _endpoint;
        private bool _disposedValue = false;
        private bool _disposed;
        private readonly IpcResponse _response;

        private EventPipeSession(IpcEndpoint endpoint, IpcResponse response, long sessionId)
        {
            _endpoint = endpoint;
            _response = response;
            _sessionId = sessionId;
        }

        public Stream EventStream => _response.Continuation;

        internal static EventPipeSession Start(IpcEndpoint endpoint, IEnumerable<EventPipeProvider> providers, bool requestRundown, int circularBufferMB)
        {
            IpcMessage requestMessage = CreateStartMessage(providers, requestRundown, circularBufferMB);
            IpcResponse? response = IpcClient.Sen
        }

        private static IpcMessage CreateStartMessage(IEnumerable<EventPipeProvider> providers, bool requestRundown, int circularBufferMB)
        {
            var config = new EventPipeSessionConfiguration(circularBufferMB, EventPipeSerializationFormat.NetTrace, providers, requestRundown);
            return new IpcMessage(DiagnosticsServerCommandSet.EventPipe, (byte)EventPipeCommandId.CollectTracing2, config.SerializeV2())
        }
    }
}
