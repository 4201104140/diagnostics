// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.Common.Utils
{
    // <summary>
    // ProcessLauncher is a child-process launcher for "diagnostics tools at startup" scenarios
    // It launches the target process at startup and passes its processId to the corresponding Command handler.
    // </summary>
    internal class ProcessLauncher
    {
        private Process _childProc = null;
        internal static ProcessLauncher Launcher = new ProcessLauncher();

        public void PrepareChildProcess(string[] args)
        {
            int unparsedTokenIdx = FindUnparsedTokenIndex(args);
            if (unparsedTokenIdx < 0)
            {
                return;
            }

            _childProc = new Process();
            _childProc.StartInfo.FileName = args[unparsedTokenIdx];
            string arguments = "";
            for (int i = unparsedTokenIdx + 1; i < args.Length; i++)
            {
                if (args[i].Contains(" "))
                {
                    arguments += $"\"{args[i].Replace("\"", "\\\"")}\"";
                }
                else
                {
                    arguments += args[i];
                }

                if (i != args.Length)
                    arguments += " ";
            }
            _childProc.StartInfo.Arguments = arguments;
        }

        private int FindUnparsedTokenIndex(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--" && i < (args.Length - 1)) return i + 1;
            }
            return -1;
        }

        public bool HasChildProc
        {
            get
            {
                return _childProc != null;
            }
        }

        public Process ChildProc
        {
            get
            {
                return _childProc;
            }
        }
    }
}