// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Diagnostics.NETCore.Client
{
    public class GetProcessInfoTests
    {
        private readonly ITestOutputHelper output;

        public GetProcessInfoTests(ITestOutputHelper outputHelper)
        {
            output = outputHelper;
        }

        [Fact]
        public void BasicProcessInfoTest()
        {
            using TestRunner runner = new TestRunner(CommonHelper.GetTraceePathWithArgs(targetFramework: "net5.0"), output);
            runner.Start();

            try
            {
                DiagnosticsClient client = new DiagnosticsClient(runner.Pid);

                ProcessInfo processInfo = client.GetProcessInfo();
            }
        }
    }
}
