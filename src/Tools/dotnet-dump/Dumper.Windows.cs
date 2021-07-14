
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Dump
{
    public partial class Dumper
    {
        private static class Windows
        {
            internal static void CollectDump(Process process, string outputFile, DumpTypeOption type)
            {
                // Open the file for writing
                using (var stream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    NativeMethods.MINIDUMP_TYPE dumpType = NativeMethods.MINIDUMP_TYPE.MiniDumpNormal;
                    switch (type)
                    {
                        case DumpTypeOption.Full:
                            dumpType = NativeMethods.MINIDUMP_TYPE.MiniDumpWithFullMemory |
                                       NativeMethods.MINIDUMP_TYPE.MiniDumpWithDataSegs;
                            break;
                        case DumpTypeOption.Heap:
                            dumpType = NativeMethods.MINIDUMP_TYPE.MiniDumpWithDataSegs;
                            break;
                        case DumpTypeOption.Mini:
                            dumpType = NativeMethods.MINIDUMP_TYPE.MiniDumpWithThreadInfo;
                            break;
                    }

                    // Retry the write dump on ERROR_PARTIAL_COPY
                    for (int i = 0; i < 5; i++)
                    {
                        // Dump the process!
                        if (NativeMethods.MiniDumpWriteDump(process.Handle, (uint)process.Id, stream.SafeFileHandle, dumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
                        {
                            break;
                        }
                        else
                        {
                            int err = Marshal.GetHRForLastWin32Error();
                            if (err != NativeMethods.ERROR_PARTIAL_COPY)
                            {
                                Marshal.ThrowExceptionForHR(err);
                            }
                        }
                    }
                }
            }

            private static class NativeMethods
            {
                public const int ERROR_PARTIAL_COPY = unchecked((int)0x8007012b);

                [DllImport("Dbghelp.dll", SetLastError = true)]
                public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, SafeFileHandle hFile, MINIDUMP_TYPE DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

                [Flags]
                public enum MINIDUMP_TYPE : uint
                {
                    MiniDumpNormal = 0,
                    MiniDumpWithDataSegs = 1 << 0,
                    MiniDumpWithFullMemory = 1 << 1,


                    MiniDumpWithThreadInfo = 1 << 12,
                }
            }
        }
    }
}
