// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tools.Counters.Exporters;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Internal.Common.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.Tools.Counters
{
    public class CounterMonitor
    {
        const int BufferDelaySecs = 1;

        private int _processId;
        private int _interval;
        private CounterSet _counterList;
        private CancellationToken _ct;
        private IConsole _console;
        private ICounterRenderer _renderer;
        private string _output;
        private bool _pauseCmdSet;
        private TaskCompletionSource<int> _shouldExit;
        private bool _resumeRuntime;
        private DiagnosticsClient _diagnosticsClient;
        private EventPipeSession _session;
        private string _metricsEventSourceSessionId;
        private int _maxTimeSeries;
        private int _maxHistograms;

        class ProviderEventState
        {
            public DateTime FirstReceiveTimestamp;
            public bool InstrumentEventObserved;
        }
        private Dictionary<string, ProviderEventState> _providerEventStates = new Dictionary<string, ProviderEventState>();
        private Queue<CounterPayload> _bufferedEvents = new Queue<CounterPayload>();

        public CounterMonitor()
        {
            _pauseCmdSet = false;
            _metricsEventSourceSessionId = Guid.NewGuid().ToString();
            _shouldExit = new TaskCompletionSource<int>();
        }

        private void DynamicAllMonitor(TraceEvent obj)
        {
            
        }

        private void StopMonitor()
        {
            try
            {
                _session?.Stop();
            }
            catch (EndOfStreamException ex)
            {
                // If the app we're monitoring exits abruptly, this may throw in which case we just swallow the exception and exit gracefully.
                Debug.WriteLine($"[ERROR] {ex.ToString()}");
            }
            // We may time out if the process ended before we sent StopTracing command. We can just exit in that case.
            catch (TimeoutException)
            {
            }
            // On Unix platforms, we may actually get a PNSE since the pipe is gone with the process, and Runtime Client Library
            // does not know how to distinguish a situation where there is no pipe to begin with, or where the process has exited
            // before dotnet-counters and got rid of a pipe that once existed.
            // Since we are catching this in StopMonitor() we know that the pipe once existed (otherwise the exception would've 
            // been thrown in StartMonitor directly)
            catch (PlatformNotSupportedException)
            {
            }
            // On non-abrupt exits, the socket may be already closed by the runtime and we won't be able to send a stop request through it. 
            catch (ServerNotAvailableException)
            {
            }
            _renderer.Stop();
        }

        public async Task<int> Monitor(
            CancellationToken ct,
            List<string> counter_list,
            string counters,
            IConsole console,
            int processId,
            int refreshInterval,
            string name,
            string diagnosticPort,
            bool resumeRuntime,
            int maxHistograms,
            int maxTimeSeries)
        {
            try
            {
                // System.CommandLine does have an option to specify argumnets are unit and it would validate they are non-negative. However the error
                // message is "cannot parse argument '-1' for option '--maxTimeServices' as expected type System.UInt32' which is not as user friendly
                // If there was another option to leverage System.Commandline that provides a little more user friendly error message we could switch
                // to it
                ValidateNonNegative(maxHistograms, nameof(maxHistograms));
                ValidateNonNegative(maxTimeSeries, nameof(maxTimeSeries));
                if (!ProcessLauncher.Launcher.HasChildProc && !CommandUtils)
            }
        }

        static private void ValidateNonNegative(int value, string argName)
        {
            if (value < 0)
            {
                throw new CommandLineErrorException($"Argument --{argName} must be non-negative");
            }
        }

        private string BuildProviderString()
        {
            string providerString;
            if (_counterList.Count == 0)
            {
                CounterProvider defaultProvider = null;
                _console.Out.WriteLine($"--counters is unspecified. Monitoring System.Runtime counters by default.");

                // Enable the default profile if nothing is specified

                providerString = defaultProvider.ToProviderString(_interval);
                filter.AddFilter("System.Runtime");
            }
            else
            {
                CounterProvider provider = null;
                StringBuilder sb = new StringBuilder("");
                for (var i = 0; i < _counterList.Count; i++)
                {
                    string counterSpecifier = _counterList[i];
                    string[] tokens = counterSpecifier.Split('[');
                    string providerName = tokens[0];
                    if (false)
                    {

                    }
                    else
                    {
                        sb.Append(provider.ToProviderString(_interval));
                    }
                    if (i != _counterList.Count - 1)
                    {
                        sb.Append(",");
                    }

                    if (tokens.Length == 1)
                    {
                        filter.AddFilter(providerName);
                    }
                    else
                    {
                        string counterNames = tokens[1];
                        string[] enabledCounters = counterNames.Substring(0, counterNames.Length - 2).Split(',');

                        filter.AddFilter(providerName, enabledCounters);
                    }
                }
                providerString = sb.ToString();
            }
            return providerString;
        }

        private async Task<int> Start()
        {
            string providerString = BuildProviderString();
            if (providerString.Length == 0)
            {
                return ReturnCode.ArgumentError;
            }

            _renderer.Initialize();

            Task monitorTask = new Task(() => {
                try
                {
                    _session = _diagnosticsClient.StartEventPipeSession(Trace.Extensions.ToProviders(providerString), false, 10);
                    if (shouldResumeRuntime)
                    {
                        _diagnosticsClient.ResumeRuntime();
                    }
                    var source = new EventPipeEventSource(_session.EventStream);
                    source.Dynamic.All += DynamicAllMonitor;
                    _renderer.EventPipeSourceConnected();
                    source.Process();
                }
                catch (DiagnosticsClientException ex)
                {
                    Console.WriteLine($"Failed to start the counter session: {ex.ToString()}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {ex.ToString()}");
                }
                finally
                {
                    shouldExit.Set();
                }
            });

            monitorTask.Start();

            while (!shouldExit.WaitOne(250))
            {
                while (true)
                {
                    if (shouldExit.WaitOne(250))
                    {
                        StopMonitor();
                        return ReturnCode.Ok;
                    }
                    if (Console.KeyAvailable)
                    {
                        break;
                    }
                }
                ConsoleKey cmd = Console.ReadKey(true).Key;
                if (cmd == ConsoleKey.Q)
                {
                    StopMonitor();
                    break;
                }
                else if (cmd == ConsoleKey.P)
                {
                    pauseCmdSet = true;
                }
                else if (cmd == ConsoleKey.R)
                {
                    pauseCmdSet = false;
                }
            }
            return await Task.FromResult(ReturnCode.Ok);
        }
    }    
}
