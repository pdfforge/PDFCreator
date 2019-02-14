using pdfforge.PDFCreator.Utilities.WindowsApi;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IKernel32
    {
        long GetSystemVolumeSerial();
    }

    public class Kernel32 : IKernel32
    {
        public long GetSystemVolumeSerial()
        {
            var volname = new StringBuilder(261);
            var fsname = new StringBuilder(261);
            uint sernum, maxlen;
            FileSystemFeature flags;
            if (!GetVolumeInformation(Environment.GetEnvironmentVariable("SYSTEMDRIVE") + "\\", volname, volname.Capacity, out sernum, out maxlen, out flags, fsname, fsname.Capacity))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            return sernum;
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetVolumeInformation(
            string rootPathName,
            StringBuilder volumeNameBuffer,
            int volumeNameSize,
            out uint volumeSerialNumber,
            out uint maximumComponentLength,
            out FileSystemFeature fileSystemFlags,
            StringBuilder fileSystemNameBuffer,
            int nFileSystemNameSize);
    }
}
