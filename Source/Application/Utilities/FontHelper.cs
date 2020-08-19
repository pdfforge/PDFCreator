using Microsoft.Win32;
using NLog;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IFontHelper
    {
        string GetFontFilename(Font font);

        string GetFontFilename(string fontFamily);
    }

    public class FontHelper : IFontHelper
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string MatchRegistryFontKey(Font font, RegistryKey fonts)
        {
            string suffix = "(?: Regular)?";
            if (font.Bold)
                suffix += "(?: Bold)?";
            if (font.Italic)
                suffix += "(?: Italic)?";

            var regex = new Regex(@"^(?:.+ & )?" + Regex.Escape(font.Name) + @"(?: & .+)?(?<suffix>" + suffix + @") \(TrueType\)$", RegexOptions.Compiled);

            string[] names = fonts.GetValueNames();

            string name = names
                .Select(n => regex.Match(n))
                .Where(m => m.Success)
                .OrderByDescending(m => m.Groups["suffix"].Length)
                .Select(m => m.Value)
                .FirstOrDefault();

            return name;
        }

        public string GetFontFilename(Font font)
        {
            RegistryKey[] registryKeys =
            {
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", false),
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", false),
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Fonts", false)
            };

            try
            {
                foreach (RegistryKey fonts in registryKeys)
                {
                    if (fonts != null)
                    {
                        var name = MatchRegistryFontKey(font, fonts);
                        if (!string.IsNullOrEmpty(name))
                        {
                            return PathSafe.GetFileName(fonts.GetValue(name).ToString());
                        }
                    }
                }

                _logger.Warn($"Incompatible Font: {font.Name} {font.Style.ToString()}");

                return null;
            }
            finally
            {
                foreach (RegistryKey key in registryKeys)
                {
                    key?.Dispose();
                }
            }
        }

        public string GetFontFilename(string fontFamily)
        {
            return GetFontFilename(new Font(fontFamily, 14));
        }
    }
}
