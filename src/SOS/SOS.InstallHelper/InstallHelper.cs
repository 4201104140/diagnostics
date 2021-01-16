using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SOS
{
    /// <summary>
    /// Functions to install and configure SOS from the package containing this code.
    /// </summary>
    public sealed class InstallHelper
    {
        /// <summary>
        /// Well known location to install SOS. Defaults to $HOME/.dotnet/sos on xplat and %USERPROFILE%/.dotnet/sos on Windows.
        /// </summary>
        public string InstallLocation { get; set; }

        /// <summary>
        /// On Linux/MacOS, the location of the lldb ".lldbinit" file. Defaults to $HOME/.lldbinit.
        /// </summary>
        public string LLDBInitFile { get; set; }

        /// <summary>
        /// If true, enable the symbol server support when configuring lldb.
        /// </summary>
        public bool EnableSymbolServer { get; set; } = true;

        /// <summary>
        /// The source path from which SOS is installed. Default is OS/architecture (RID) named directory in the same directory as this assembly.
        /// </summary>
        public string SOSSourcePath { get; set; }

        /// <summary>
        /// Console output delegate
        /// </summary>
        private readonly Action<string> m_writeLine;

        /// <summary>
        /// Create an instance of the installer.
        /// </summary>
        /// <param name="writeLine">console output delegate</param>
        /// <param name="architecture">architecture to install or if null using the current process architecture</param>
        /// <exception cref="SOSInstallerException">environment variable not found</exception>
        public InstallHelper(Action<string> writeLine, Architecture? architecture = null)
        {
            m_writeLine = writeLine;
            string rid = GetRid(architecture);
            string home;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                home = Environment.GetEnvironmentVariable("USERPROFILE");
                if (string.IsNullOrEmpty(home))
                {
                    throw new SOSInstallerException("USERPROFILE environment variable not found");
                }
            }
            else
            {
                home = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(home))
                {
                    throw new SOSInstallerException("HOME environment variable not found");
                }
                LLDBInitFile = Path.Combine(home, ".lldbinit");
            }
            InstallLocation = Path.GetFullPath(Path.Combine(home, ".dotnet", "sos"));
            SOSSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), rid);
        }

        /// <summary>
        /// Install SOS to well known location (InstallLocation).
        /// </summary>
        /// <exception cref="SOSInstallerException">various</exception>
        public void Install()
        {
            WriteLine("Installing SOS to {0} from {1}", InstallLocation, SOSSourcePath);

            if (string.IsNullOrEmpty(SOSSourcePath))
            {
                throw new SOSInstallerException("SOS source path not valid");
            }
            if (!Directory.Exists(SOSSourcePath))
            {
                throw new SOSInstallerException($"Operating system or architecture not supported: installing from {SOSSourcePath}");
            }
            if (string.IsNullOrEmpty(InstallLocation))
            {
                throw new SOSInstallerException($"Installation path {InstallLocation} not valid");
            }

            
        }

        /// <summary>
        /// Uninstalls and removes the SOS configuration.
        /// </summary>
        /// <exception cref="SOSInstallerException">various</exception>
        public void Uninstall()
        {
            WriteLine("Uninstalling SOS from {0}", InstallLocation);
            if (string.IsNullOrEmpty(LLDBInitFile))
            {

            }
            if (Directory.Exists(InstallLocation))
            {

            }
            else
            {
                WriteLine("SOS not installed");
            }
        }

        /// <summary>
        /// Returns the RID
        /// </summary>
        /// <param name="architecture">architecture to install or if null using the current process architecture</param>
        public static string GetRid(Architecture? architecture = null)
        {
            string os = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "win";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "osx";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "linux";
                try
                {
                    string ostype = File.ReadAllText("/etc/os-release");
                    if (ostype.Contains("ID=alpine"))
                    {
                        os = "linux-musl";
                    }
                }
                catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is IOException)
                {
                }
            }
            if (os == null)
            {
                throw new SOSInstallerException($"Unsupported operating system {RuntimeInformation.OSDescription}");
            }
            string architectureString = (architecture.HasValue ? architecture : RuntimeInformation.ProcessArchitecture).ToString().ToLowerInvariant();
            return $"{os}-{architectureString}";
        }

        private void WriteLine(string format, params object[] args)
        {
            m_writeLine?.Invoke(string.Format(format, args));
        }
    }

    /// <summary>
    /// SOS installer error
    /// </summary>
    public class SOSInstallerException : Exception
    {
        public SOSInstallerException(string message)
            : base(message)
        {
        }

        public SOSInstallerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
