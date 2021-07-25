// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Diagnostics.ExtensionCommands;
using Microsoft.Diagnostics.DebugServices;
using Microsoft.Diagnostics.DebugServices.Implementation;
using Microsoft.Diagnostics.Repl;
using Microsoft.Diagnostics.Runtime;
using SOS.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Microsoft.Diagnostics.Tools.Dump
{
    public class Analyzer : IHost
    {
        private readonly ServiceProvider _serviceProvider;

        private readonly ContextService _contextService;
        private int _targetIdFactory;
        private Target _target;

        public Analyzer()
        {
            _serviceProvider = new ServiceProvider();

            _contextService = new ContextService(this);

            _serviceProvider.AddService<IHost>(this);
            _serviceProvider.AddService<IContextService>(_contextService);

            
            
        }

        public Task<int> Analyze(FileInfo dump_path, string[] command)
        {

            // Attempt to load the persisted command history
            string dotnetHome;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dotnetHome = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".dotnet");
            }
            else
            {
                dotnetHome = Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".dotnet");
            }
            string historyFileName = Path.Combine(dotnetHome, "dotnet-dump.history");
            try
            {
                string[] history = File.ReadAllLines(historyFileName);
            }
            catch (Exception ex) when
                (ex is IOException ||
                 ex is UnauthorizedAccessException ||
                 ex is NotSupportedException ||
                 ex is SecurityException)
            {
            }

            try
            {
                using DataTarget dataTarget = DataTarget.LoadDump(dump_path.FullName);

                OSPlatform targetPlatform = dataTarget.DataReader.TargetPlatform;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || dataTarget.DataReader.EnumerateModules().Any((module) => Path.GetExtension(module.FileName) == ".dylib"))
                {
                    targetPlatform = OSPlatform.OSX;
                }
                _target = new TargetFromDataReader
            }
        }

        #region IHost

        public IServiceEvent OnShutdownEvent { get; } = new ServiceEvent();

        HostType IHost.HostType => HostType.DotnetDump;

        IServiceProvider IHost.Services => _serviceProvider;

        IEnumerable<ITarget> IHost.EnumerateTargets() => _target != null ? new ITarget[] { _target } : Array.Empty<ITarget>();

        public void DestroyTarget(ITarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (target == _target)
            {
                _target = null;
                _con
            }
        }
        #endregion
    }
}
