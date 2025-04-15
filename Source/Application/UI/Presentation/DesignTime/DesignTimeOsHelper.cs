using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeOsHelper : IOsHelper
    {
        public bool Is64BitProcess { get; } = false;
        public bool Is64BitOperatingSystem { get; } = false;

        public string WindowsFontsFolder { get; } = "Fonts";

        public bool UserIsAdministrator()
        {
            return false;
        }

        public string GetWindowsVersion()
        {
            return "WinXP";
        }

        public void AddDllDirectorySearchPath(string path)
        { }
    }
}
