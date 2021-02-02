using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Diagnostics.NETCore.Client
{
    public class EventPipeSessionTests
    {
        private readonly ITestOutputHelper output;

        public EventPipeSessionTests(ITestOutputHelper outputHelper)
        {
            output = outputHelper;
        }

        /// <summary>
        /// A simple test that checks if we can create an EventPipeSession on a child process
        /// </summary>
        [Fact]
        public void BasicEventPipeSessionTest()
        {
            using TestRunner runner = new TestRunner(CommonHelper.GetTraceePathWithArgs(), output);
            runner.Start(timeoutInMSPipeCreation: 15_000, testProcessTimeout: 60_000);
        }
    }
}
