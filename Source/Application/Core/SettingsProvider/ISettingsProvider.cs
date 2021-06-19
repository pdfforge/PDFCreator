using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.Core.SettingsProvider
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
