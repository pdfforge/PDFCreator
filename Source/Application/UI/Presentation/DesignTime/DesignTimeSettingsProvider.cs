using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public partial class DesignTimePrintJobViewModel : PrintJobViewModel
    {
        public class DesignTimeSettingsProvider : ISettingsProvider
        {
            public DesignTimeSettingsProvider()
            {
                Settings = new PdfCreatorSettings();
                Settings.ConversionProfiles.Add(new ConversionProfile { Name = "Default profile" });
            }

#pragma warning disable CS0067

            public event EventHandler<LanguageChangedEventArgs> LanguageChanged;

#pragma warning restore CS0067

            public string GetApplicationLanguage()
            {
                return "en";
            }

            public PdfCreatorSettings Settings { get; }

            public ConversionProfile GetDefaultProfile()
            {
                return Settings.ConversionProfiles.First();
            }

            public bool CheckValidSettings(PdfCreatorSettings settings)
            {
                return true;
            }

            public void UpdateSettings(PdfCreatorSettings settings)
            {
                throw new NotImplementedException();
            }

#pragma warning disable 67

            public event EventHandler SettingsChanged;

#pragma warning restore 67
        }
    }
}
