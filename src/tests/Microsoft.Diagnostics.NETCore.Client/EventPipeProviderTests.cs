// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using Xunit;


namespace Microsoft.Diagnostics.NETCore.Client
{
    /// <summary>
    /// Suite of tests that test top-level commands
    /// </summary>
    public class EventPipeProviderTests
    {
        [Fact]
        public void EqualTest1()
        {
            EventPipeProvider provider1 = new EventPipeProvider("myProvider", EventLevel.Informational);
            EventPipeProvider provider2 = new EventPipeProvider("myProvider", EventLevel.Informational);
            Assert.True(provider1 == provider2);
        }
    }
}
