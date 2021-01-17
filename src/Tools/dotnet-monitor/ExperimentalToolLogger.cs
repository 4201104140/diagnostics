using Microsoft.Extensions.Logging;

namespace Microsoft.Diagnostics.Tools.Monitor
{
    // FUTURE: This log message should be removed when dotnet-monitor is no longer an experimental tool
    internal class ExperimentalToolLogger
    {
        private const string ExperimentMessage = "WARNING: dotnet-monitor is experimental and is not intended for production environments yet.";

        private readonly ILogger<ExperimentalToolLogger> _logger;

        public ExperimentalToolLogger(ILogger<ExperimentalToolLogger> logger)
        {
            _logger = logger;
        }

        public void LogExperimentMessage()
        {
            _logger.LogWarning(ExperimentMessage);
        }

        public static void AddLogFilter(ILoggingBuilder builder)
        {
            builder.AddFilter(typeof(ExperimentalToolLogger).FullName, LogLevel.Warning);
        }
    }
}