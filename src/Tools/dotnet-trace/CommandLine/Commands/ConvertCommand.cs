using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;

namespace Microsoft.Diagnostics.Tools.Trace
{
    internal sealed class ConvertCommandHandler
    {
        public static int ConvertFile(IConsole console, FileInfo inputFilename, TraceFileFormat format, FileInfo output)
        {
            if ((int)format <= 0)
            {
                Console.Error.WriteLine("--format is required.");
                return ErrorCodes.ArgumentError;
            }

            if (format == TraceFileFormat.NetTrace)
            {
                Console.Error.WriteLine("Cannot convert a nettrace file to nettrace format.");
                return ErrorCodes.ArgumentError;
            }

            if (!inputFilename.Exists)
            {
                Console.Error.WriteLine($"File '{inputFilename}' does not exist.");
            }

            if (output == null)
                output = inputFilename;

            return 0;
        }

        public static Command ConvertCommand() =>
            new Command(
                name: "convert",
                description: "Comverts traces to alternate formats for use with alternate trace analysis tools. Can only convert from the nettrace format")
            {

            };
    }
}
