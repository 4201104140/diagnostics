using System;

namespace Microsoft.Diagnostics.NETCore.Client
{
    public class DiagnosticsClientException : Exception
    {
        public DiagnosticsClientException(string msg) : base(msg) { }
    }

    public class UnsupportedProtocolException : DiagnosticsClientExceptions
    {
        public UnsupportedProtocolException(string msg) : base(msg) {}
    }

}
