using Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using System;
using System.IO;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IPdfEditorHelper
    {
        bool UseSodaPdf { get; }
    }

    public class PdfEditorHelper : IPdfEditorHelper
    {
        private readonly Lazy<bool> _useSodaPdf;

        public bool UseSodaPdf => _useSodaPdf.Value;

        public PdfEditorHelper(IInstallationPathProvider installationPathProvider, EditionHelper editionHelper)
        {
            if (editionHelper.AlwaysUsePdfArchitect)
            {
                _useSodaPdf = new Lazy<bool>(() => false);
            }
            else
            {
                _useSodaPdf = new Lazy<bool>(() =>
                {
                    var path = Path.Combine("HKEY_LOCAL_MACHINE", installationPathProvider.ApplicationRegistryPath, "Parameters");
                    var value = Registry.GetValue(path, "PDFEditor", "") as string ?? "";

                    return value.ToLowerInvariant().Contains("soda");
                });
            }
        }
    }
}
