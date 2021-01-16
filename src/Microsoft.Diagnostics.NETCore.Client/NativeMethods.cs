using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Diagnostics.NETCore.Client
{
    internal class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekNamedPipe(
            SafePipeHandle hNamedPipe,
            byte[] lpBuffer,
            int bufferSize,
            IntPtr lpBytesRead,
            IntPtr lpTotalBytesAvail,
            IntPtr lpBytesLeftThisMessage
            );
    }
}