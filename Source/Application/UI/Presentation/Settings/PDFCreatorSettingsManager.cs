using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;

namespace pdfforge.PDFCreator.UI.Presentation.Settings
{
    public class PDFCreatorSettingsManager : SettingsManager
    {
        public PDFCreatorSettingsManager(SettingsProvider settingsProvider, ISettingsLoader loader, IInstallationPathProvider installationPathProvider) 
            : base(settingsProvider, loader, installationPathProvider)
        {
        }

        protected override void PreSaveHook(PdfCreatorSettings settings)
        {
        }

        protected override void PostSaveHook(PdfCreatorSettings settings)
        {
            LoggingHelper.ChangeLogLevel(SettingsProvider.Settings.ApplicationSettings.LoggingLevel);
        }

        protected override void PostLoadHook(PdfCreatorSettings settings)
        {
            LoggingHelper.ChangeLogLevel(SettingsProvider.Settings.ApplicationSettings.LoggingLevel);
        }
    }
}