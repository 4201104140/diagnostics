using Microsoft.Internal.Common.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

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
                    // Build timestamp file path
                    string timestamp = $"{DateTime.Now:yyyyMMdd_Hh}";

                }
            }
        }
    }
}
