
using Microsoft.Diagnostics.DebugServices;
using Microsoft.Diagnostics.Runtime.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SOS.Hosting
{
    public sealed class SOSLibrary
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int SOSInitializeDelegate(
            IntPtr IHost);

        private readonly IContextService _contextService;

        private IntPtr _sosLibrary = IntPtr.Zero;

        /// <summary>
        /// The native SOS binaries path. Default is OS/architecture (RID) named directory in the same directory as this assembly.
        /// </summary>
        public string SOSPath { get; set; }

        public static SOSLibrary Create(IHost host)
        {
            SOSLibrary sosLibrary = null;
            try
            {
                sosLibrary = new SOSLibrary(host);
                sosLibrary.Initialize();
            }
            catch
            {
                sosLibrary = null;
                throw;
            }
            return sosLibrary;
        }

        private SOSLibrary(IHost host)
        {
            _contextService = host.Services.GetService<IContextService>();

            string rid = InstallHelper.GetRid();
            SOSPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), rid);


        }

        private void Initialize()
        {
            if (_sosLibrary == IntPtr.Zero)
            {
                string sos;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    sos = "sos.dll";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    sos = "libsos.so";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    sos = "libsos.dylib";
                }
                else
                {
                    throw new PlatformNotSupportedException($"Unsupported operating system: {RuntimeInformation.OSDescription}");
                }
                string sosPath = Path.Combine(SOSPath, sos);
                try
                {
                    _sosLibrary = Microsoft.Diagnostics.Runtime.DataTarget.PlatformFunctions.LoadLibrary(sosPath);
                }
                catch (DllNotFoundException ex)
                {
                    // This is a workaround for the Microsoft SDK docker images. Can fail when LoadLibrary uses libdl.so to load the SOS module.
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        throw new DllNotFoundException("Problem loading SOS module. Try installing libc6-dev (apt-get install libc6-dev) to work around this problem.", ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                if (_sosLibrary == IntPtr.Zero)
                {
                    throw new FileNotFoundException($"SOS module {sosPath} not found");
                }
                //var initializeFunc = Sos
            }
        }
    }
}
