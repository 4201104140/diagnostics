using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Diagnostics.NETCore.Client
{
    internal class ProcessInfo
    {
        private static readonly int GuidSizeInBytes = 16;

        public static ProcessInfo Parse(byte[] payload)
        {
            ProcessInfo processInfo = new ProcessInfo();

            int index = 0;
            processInfo
        }

        private static string ReadString(byte[] buffer, ref int index)
        {
            // Length of the string of UTF-16 characters.
            int length = (int)BitConverter
        }

        public UInt64 ProcessId { get; private set; }
        public Guid RuntimeInstanceCookie { get; private set; }
        public string CommandLine { get; private set; }
        public string OperatingSystem { get; private set; }
        public string ProcessArchitecture { get; private set; }
    }
}
