using System;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagementInterface
{
    public interface ISettingsProvider : IApplicationLanguageProvider
    {
        PdfCreatorSettings Settings { get; }

        ConversionProfile GetDefaultProfile();

        bool CheckValidSettings(PdfCreatorSettings settings);

        void UpdateSettings(PdfCreatorSettings settings);

        event EventHandler SettingsChanged;
    }

    public interface IApplicationLanguageProvider
    {
        event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        string GetApplicationLanguage();
    }
}
