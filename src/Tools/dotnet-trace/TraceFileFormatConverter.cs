using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Diagnostics.Symbols;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Stacks;
using Microsoft.Diagnostics.Tracing.Stacks.Formats;

namespace Microsoft.Diagnostics.Tools.Trace
{
    internal enum TraceFileFormat { NetTrace, Speedscope, Chromium };

    internal static class TraceFileFormatConverter
    {
        private static IReadOnlyDictionary<TraceFileFormat, string> TraceFileFormatExtensions = new Dictionary<TraceFileFormat, string>()
        {
            { TraceFileFormat.NetTrace, "nettrace" },
            { TraceFileFormat.Speedscope, "speedscope.json"},
            { TraceFileFormat.Chromium, "chromium.json" }
        };

        public static void ConvertToFormat(TraceFileFormat format, string fileToConvert, string outputFilename = "")
        {
            if (string.IsNullOrWhiteSpace(outputFilename))
                outputFilename = fileToConvert;

            outputFilename = Path.ChangeExtension(outputFilename, TraceFileFormatExtensions[format]);
            Console.Out.WriteLine($"Writing:\t{outputFilename}");

            switch (format)
            {
                case TraceFileFormat.NetTrace:
                    break;
                case TraceFileFormat.Speedscope:
                case TraceFileFormat.Chromium:
                    try
                    {
                        Convert(format, )
                    }
            }
        }

        private static void Convert(TraceFileFormat format, string fileToConvert, string outputFilename, bool continueOnError = false)
        {
            var etlxFilePath = TraceLog.CreateFromEventPipeDataFile(fileToConvert, null, new TraceLogOptions() { ContinueOnError = continueOnError });
            using (var symbolReader = new SymbolReader(TextWriter.Null) { SymbolPath = SymbolPath.MicrosoftSymbolServerPath})
            using (var eventLog = new TraceLog(etlxFilePath))
            {
                var stackSource = new MutableTraceEventStackSource
            }
        }
    }
}
