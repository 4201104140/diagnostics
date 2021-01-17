namespace Microsoft.Diagnostics.Tools.Monitor
{
    public class DiagnosticPortOptions
    {
        public const string ConfigurationKey = "DiagnosticPort";

        public DiagnosticPortConnectionMode ConnectionMode { get; set; }

        public string EndpointName { get; set; }

        public int? MaxConnections { get; set; }
    }

    public enum DiagnosticPortConnectionMode
    {
        Connect,
        Listen
    }
}
