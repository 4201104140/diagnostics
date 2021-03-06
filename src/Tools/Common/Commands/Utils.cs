

using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Internal.Common.Utils
{
    internal class CommandUtils
    {
        // Returns processId that matches the given name.
        // It also checks whether the process has a diagnostics server port.
        // If there are more than 1 process with the given name or there isn't any active process
        // with the given name, then this returns -1
        public static int FindProcessIdWithName(string name)
        {
            var publishedProcessesPids = new List<int>(DiagnosticsClient.GetPublishedProcesses());
            var processesWithMatchingName = Process.GetProcessesByName(name);
            var commonId = -1;

            for (int i = 0; i < processesWithMatchingName.Length; i++)
            {
                if (publishedProcessesPids.Contains(processesWithMatchingName[i].Id))
                {
                    if (commonId != -1)
                    {
                        Console.WriteLine("There are more than one active processes with the given name: {0}", name);
                        return -1;
                    }
                    commonId = processesWithMatchingName[i].Id;
                }
            }
            if (commonId == -1)
            {
                Console.WriteLine("There is no active process with the given name: {0}", name);
            }
            return commonId;
        }

        /// <summary>
        /// A helper method for validating --process-id, --name, --diagnostic-port options for collect with child process commands.
        /// None of these options can be specified, so it checks for them and prints the appropriate error message.
        /// </summary>
        /// <param name="processId">process ID</param>
        /// <param name="name">name</param>
        /// <param name="port">port</param>
        /// <returns></returns>
        public static bool ValidateArgumentsForChildProcess(int processId, string name, string port)
        {
            if (processId != 0 && name != null && !string.IsNullOrEmpty(port))
            {
                Console.WriteLine("None of the --name, --process-id, or --diagnostic-port options may be specified when launching a child process.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// A helper method for validating --process-id, --name, --diagnostic-port options for collect commands.
        /// Only one of these options can be specified, so it checks for duplicate options specified and if there is 
        /// such duplication, it prints the appropriate error message.
        /// </summary>
        /// <param name="processId">process ID</param>
        /// <param name="name">name</param>
        /// <param name="port">port</param>
        /// <param name="resolvedProcessId">resolvedProcessId</param>
        /// <returns></returns>
        public static bool ValidateArgumentsForAttach(int processId, string name, string port, out int resolvedProcessId)
        {
            resolvedProcessId = -1;
            if (processId == 0 && name == null && string.IsNullOrEmpty(port))
            {
                Console.WriteLine("Must specify either --process-id, --name, or --diagnostic-port.");
                return false;
            }
            else if (processId < 0)
            {
                Console.WriteLine($"{processId} is not a valid process ID");
                return false;
            }
            else if (processId != 0 && name != null && !string.IsNullOrEmpty(port))
            {
                Console.WriteLine("Only one of the --name, --process-id, or --diagnostic-port options may be specified.");
                return false;
            }
            else if (processId != 0 && name != null)
            {
                Console.WriteLine("Can only one of specify --name or --process-id.");
                return false;
            }
            else if (processId != 0 && !string.IsNullOrEmpty(port))
            {
                Console.WriteLine("Can only one of specify --process-id or --diagnostic-port.");
                return false;
            }
            else if (name != null && !string.IsNullOrEmpty(port))
            {
                Console.WriteLine("Can only one of specify --name or --diagnostic-port.");
                return false;
            }
            // If we got this far it means only one of --name/--diagnostic-port/--process-id was specified
            else if (!string.IsNullOrEmpty(port))
            {
                return true;
            }
            // Resolve name option
            else if (name != null)
            {
                processId = CommandUtils.FindProcessIdWithName(name);
                if (processId < 0)
                {
                    return false;
                }
            }
            else if (processId == 0)
            {
                Console.WriteLine("One of the --name, --process-id, or --diagnostic-port options must be specified when attaching to a process.");
                return false;
            }
            resolvedProcessId = processId;
            return true;
        }
    }

    internal class ReturnCode
    {
        public static int Ok = 0;
        public static int SessionCreationError = 1;
        public static int TracingError = 2;
        public static int ArgumentError = 3;
        public static int UnknownError = 4;
    }
}