// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Microsoft.Diagnostics.Tracing.Parsers;

namespace Microsoft.Diagnostics.Tools.Counters
{
    internal static class KnownData
    {
        private static readonly string maxVersion = "5.0";
        private static readonly IReadOnlyDictionary<string, CounterProvider> _knownProviders;

        private static readonly string net60 = "6.0";
        private static readonly string net50 = "5.0";
        private static readonly string net31 = "3.1";
        private static readonly string net30 = "3.0";

        private static IEnumerable<CounterProvider> CreateKnownProviders(string runtimeVersion)
        {
            yield return new CounterProvider(
                "System.Runtime", // Name
                "A default set of performance counters provided by the .NET runtime.", // Description
                "0xffffffff", // Keywords
                "5", // Level 
                new[] { // Counters
                    new CounterProfile{ Name="cpu-usage", Description="The percent of process' CPU usage relative to all of the system CPU resources [0-100]", SupportedVersions=new[] { net30, net31, net50 } },
                },
                runtimeVersion //
            );
            yield return new CounterProvider(
                "Microsoft.AspNetCore.Hosting", // Name
                "A set of performance counters provided by ASP.NET Core.", // Description
                "0x0", // Keywords
                "4", // Level 
                new[] { // Counters
                    new CounterProfile{ Name="requests-per-second", Description="Number of requests between update intervals", SupportedVersions=new[] { net30, net31, net50 } },
                    new CounterProfile{ Name="total-requests", Description="Total number of requests", SupportedVersions=new[] { net30, net31, net50 } },
                    new CounterProfile{ Name="current-requests", Description="Current number of requests", SupportedVersions=new[] { net30, net31, net50 } },
                    new CounterProfile{ Name="failed-requests", Description="Failed number of requests", SupportedVersions=new[] { net30, net31, net50 } },
                },
                runtimeVersion
            );
        }

        public static IReadOnlyList<CounterProvider> GetAllProviders(string version)
        {
            return CreateKnownProviders(version).Where(p => p.Counters.Count > 0).ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase).Values.ToList();
        }

        public static bool TryGetProvider(string providerName, out CounterProvider provider) => _knownProviders.TryGetValue(providerName, out provider);
    }
}
