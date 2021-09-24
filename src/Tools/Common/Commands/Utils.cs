// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Internal.Common.Utils
{
    internal class ReturnCode
    {
        public static int Ok = 0;
        public static int SessionCreationError = 1;
        public static int TracingError = 2;
        public static int ArgumentError = 3;
        public static int UnknownError = 4;
    }
}