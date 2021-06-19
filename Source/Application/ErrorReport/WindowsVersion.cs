using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.ErrorReport
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RTL_OSVERSIONINFOEX
    {
        internal uint dwOSVersionInfoSize;
        internal uint dwMajorVersion;
        internal uint dwMinorVersion;
        internal uint dwBuildNumber;
        internal uint dwPlatformId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string szCSDVersion;
    }

    internal static class WindowsVersion
    {
        // borrowed from https://github.com/dotnet/core-setup/blob/6887ab556bc8302390782711dcdd75b75e769cf5/src/managed/Microsoft.DotNet.PlatformAbstractions/Native/NativeMethods.Windows.cs
        // This call avoids the shimming Windows does to report old versions
        [DllImport("ntdll")]
        private static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

        internal static string RtlGetVersion()
        {
            var osvi = new RTL_OSVERSIONINFOEX();
            osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
            if (RtlGetVersion(out osvi) == 0)
            {
                return $"{osvi.dwMajorVersion}.{osvi.dwMinorVersion}.{osvi.dwBuildNumber}";
            }

            return null;
        }
    }
}
