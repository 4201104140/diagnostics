
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Internal.Common.Utils;
using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Tools.Dump
{
    public partial class Dumper
    {
        /// <summary>
        /// The dump type determines the kinds of information that are collected from the process.
        /// </summary>
        public enum DumpTypeOption
        {
            Full,       // The largest dump containing all memory including the module images.

            Heap,       // A large and relatively comprehensive dump containing module lists, thread lists, all 
                        // stacks, exception information, handle information, and all memory except for mapped images.

            Mini,       // A small dump containing module lists, thread lists, exception information and all stacks.
        }

        public Dumper()
        {
        }

        public int Collect(IConsole console, int processId, string output, bool diag, DumpTypeOption type, string name)
        {
            Console.WriteLine(name);
            if (name != null)
            {
                if (processId != 0)
                {
                    Console.WriteLine("Can only specified either --name or --process-id options");
                    return 0;
                }
                processId = CommandUtils.FindProcessIdWithName(name);
                if (processId < 0)
                {
                    return 0;
                }
            }

            if (processId == 0)
            {
                Console.Error.WriteLine("ProcessId is required");
                return 1;
            }

            try
            {
                if (output == null)
                {
                    // Build timestamp based file path
                    string timestamp = $"{DateTime.Now:yyyyMMdd_HHmmss}";
                    output = Path.Combine(Directory.GetCurrentDirectory(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"dump_{timestamp}.dmp" : $"core_{timestamp}");
                }
                // Make sure the dump path is NOT relative. This path could be sent to the runtime
                // process on Linux which may have a different current directory.
                output = Path.GetFullPath(output);

                // Display the type of dump and dump path
                string dumpTypeMessage = null;
                switch (type)
                {
                    case DumpTypeOption.Full:
                        dumpTypeMessage = "full";
                        break;
                    case DumpTypeOption.Heap:
                        dumpTypeMessage = "dump with heap";
                        break;
                    case DumpTypeOption.Mini:
                        dumpTypeMessage = "dump";
                        break;
                }
                console.Out.WriteLine($"Writing {dumpTypeMessage} to output");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Get the process
                    Process process = Process.GetProcessById(processId);

                    Windows.CollectDump(process, output, type);
                }
                else
                {
                    
                }
            }
            catch (Exception ex) when
                (ex is FileNotFoundException ||
                 ex is DirectoryNotFoundException ||
                 ex is UnauthorizedAccessException ||
                 ex is PlatformNotSupportedException ||
                 ex is UnsupportedCommandException ||
                 ex is InvalidDataException ||
                 ex is InvalidOperationException ||
                 ex is NotSupportedException ||
                 ex is DiagnosticsClientException)
            {
                console.Error.WriteLine($"{ex.Message}");
                return 1;
            }

            console.Out.WriteLine($"Complete");
            return 0;
        }
    }
}
