using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IOsHelper
    {
        /// <summary>
        ///     Detect if the current process is running in 64-Bit mode
        /// </summary>
        bool Is64BitProcess { get; }

        /// <summary>
        ///     Detect if the application is run on a 64-Bit Windows edition
        /// </summary>
        bool Is64BitOperatingSystem { get; }

        string WindowsFontsFolder { get; }

        bool UserIsAdministrator();

        string GetWindowsVersion();

        void AddDllDirectorySearchPath(string path);
    }

    public class OsHelper : IOsHelper
    {
        /// <summary>
        ///     Detect if the current process is running in 64-Bit mode
        /// </summary>
        public bool Is64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }

        /// <summary>
        ///     Detect if the application is run on a 64-Bit Windows edition
        /// </summary>
        public bool Is64BitOperatingSystem
        {
            get { return Is64BitProcess || InternalCheckIsWow64(); }
        }

        private string _windowsVersion;

        /// <summary>
        ///     Get the Windows edition
        /// </summary>
        public string GetWindowsVersion()
        {
            if (string.IsNullOrEmpty(_windowsVersion))
            {
                _windowsVersion = Environment.OSVersion.ToString();

                try
                {
                    string osName = null;
                    var wmi = new ManagementObjectSearcher("select Caption from Win32_OperatingSystem");

                    foreach (var obj in wmi.Get())
                    {
                        osName = obj["Caption"] as string;
                        break;
                    }

                    if (osName != null)
                        _windowsVersion = osName + " (" + _windowsVersion + ")";
                }
                catch
                {
                }
            }

            return _windowsVersion;
        }

        public bool UserIsAdministrator()
        {
            try
            {
                //get the currently logged in user
                var user = WindowsIdentity.GetCurrent();
                if (user == null)
                    return false;

                var principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string WindowsFontsFolder
        {
            get
            {
                var windir = Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows";

                return Path.Combine(windir, "Fonts");
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
            );

        private bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (var p = System.Diagnostics.Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            return false;
        }

        [DllImport("kernel32.dll", CharSet =
            System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern bool SetDllDirectory(string path);

        public void AddDllDirectorySearchPath(string path)
        {
            SetDllDirectory(path);
        }
    }
}
