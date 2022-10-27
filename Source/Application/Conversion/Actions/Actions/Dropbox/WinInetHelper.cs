using System;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public interface IWinInetHelper
    {
        bool EndBrowserSession();
    }

    public class WinInetHelper : IWinInetHelper
    {
        public bool EndBrowserSession()
        {
            // 42 = INTERNET_OPTION_END_BROWSER_SESSION
            return SetOption(42, null);
        }

        private bool SetOption(int settingCode, int? option)
        {
            IntPtr optionPtr = IntPtr.Zero;
            int size = 0;
            if (option.HasValue)
            {
                size = sizeof(int);
                optionPtr = Marshal.AllocCoTaskMem(size);
                Marshal.WriteInt32(optionPtr, option.Value);
            }

            bool success = InternetSetOption(0, settingCode, optionPtr, size);

            if (optionPtr != IntPtr.Zero) Marshal.Release(optionPtr);
            return success;
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetOption(
            int hInternet,
            int dwOption,
            IntPtr lpBuffer,
            int dwBufferLength
        );
    }
}