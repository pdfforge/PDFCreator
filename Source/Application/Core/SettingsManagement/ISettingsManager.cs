using System;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsManager
    {
        ISettingsProvider GetSettingsProvider();

        void SaveCurrentSettings();

        void ApplyAndSaveSettings(PdfCreatorSettings settings);

        void LoadAllSettings();

        event EventHandler SettingsSaved;
    }
}