using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;

namespace pdfforge.PDFCreator.UI.Presentation.Settings
{
    public class PDFCreatorSettingsManager : SettingsManager
    {

        public PDFCreatorSettingsManager(SettingsProvider settingsProvider, ISettingsLoader loader, IInstallationPathProvider installationPathProvider) : base(settingsProvider, loader, installationPathProvider)
        {
        }

        protected override void ProcessAfterLoading(PdfCreatorSettings settings)
        {
            LoggingHelper.ChangeLogLevel(SettingsProvider.Settings.ApplicationSettings.LoggingLevel);
        }

        protected override void ProcessBeforeSaving(PdfCreatorSettings settings) { }

        protected override void ProcessAfterSaving(PdfCreatorSettings settings)
        {
            LoggingHelper.ChangeLogLevel(SettingsProvider.Settings.ApplicationSettings.LoggingLevel);
        }
    }
}