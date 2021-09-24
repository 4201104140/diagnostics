// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client
{
    /// <summary>
    /// This is a top-level class that contains methods to send various diagnostics command to the runtime.
    /// </summary>
    public sealed class DiagnosticsClient
    {

        /// <summary>
        /// Get all the active processes that can be attached to.
        /// </summary>
        /// <returns>
        /// IEnumerable of all the active process IDs.
        /// </returns>
        public static IEnumerable<int> GetPublishedProcesses()
        {
            static IEnumerable<int> GetAllPublishedProcesses()
            {
                foreach (var port in Directory.GetFiles(PidIpcEndpoint.IpcRootPath))
                {
                    var fileName = new FileInfo(port).Name;
                    var match = Regex.Match(fileName, PidIpcEndpoint.DiagnosticsPortPattern);
                    if (!match.Success) continue;
                    var group = match.Groups[1].Value;
                    if (!int.TryParse(group, NumberStyles.Integer, CultureInfo.InvariantCulture, out var processId))
                        continue;

                    yield return processId;
                }
            }

            return GetAllPublishedProcesses().Distinct();
        }
    }
}
