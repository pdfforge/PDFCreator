using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using System.Globalization;
using System.Linq;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class PDFCreatorSettingsLoader : SettingsLoader
    {
        private readonly IPrinterHelper _printerHelper;
        private readonly EditionHelper _editionHelper;
        private readonly IPrinterMappingsHelper _printerMappingsHelper;
        private readonly ITranslationHelper _translationHelper;

        public PDFCreatorSettingsLoader(ISettingsMover settingsMover,
            IInstallationPathProvider installationPathProvider,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IMigrationStorageFactory migrationStorageFactory,
            IActionOrderHelper actionOrderHelper,
            ISettingsBackup settingsBackup,
            ITranslationHelper translationHelper,
            IPrinterHelper printerHelper,
            EditionHelper editionHelper,
            ISharedSettingsLoader sharedSettingsLoader,
            IBaseSettingsBuilder baseSettingsBuilder,
            IGpoSettings gpoSettings,
            IPrinterMappingsHelper printerMappingsHelper)
            : base(settingsMover, installationPathProvider, defaultSettingsBuilder, migrationStorageFactory, actionOrderHelper, settingsBackup, sharedSettingsLoader, baseSettingsBuilder, gpoSettings)
        {
            _printerHelper = printerHelper;
            _editionHelper = editionHelper;
            _printerMappingsHelper = printerMappingsHelper;
            _translationHelper = translationHelper;
        }

        protected override void ProcessBeforeSaving(PdfCreatorSettings settings)
        { }

        protected override void ProcessAfterSaving(PdfCreatorSettings settings)
        { }

        protected override void PrepareForLoading()
        { }

        protected override void ProcessAfterLoading(PdfCreatorSettings settings)
        {
            _translationHelper.TranslateProfileList(settings.ConversionProfiles);
            CheckLanguage(settings);
            _printerMappingsHelper.CheckPrinterMappings(settings);
            CheckUpdateInterval(settings);
        }

        private void CheckUpdateInterval(PdfCreatorSettings settings)
        {
            if (_editionHelper.IsFreeEdition)
            {
                if (settings.ApplicationSettings.UpdateInterval == UpdateInterval.Never)
                {
                    settings.ApplicationSettings.UpdateInterval = UpdateInterval.Monthly;
                }
            }
        }

        private void CheckLanguage(PdfCreatorSettings settings)
        {
            if (!_translationHelper.HasTranslation(settings.ApplicationSettings.Language))
            {
                var language = _translationHelper.FindBestLanguage(CultureInfo.CurrentUICulture);

                var setupLanguage = _translationHelper.SetupLanguage;
                if (!string.IsNullOrWhiteSpace(setupLanguage) && _translationHelper.HasTranslation(setupLanguage))
                    language = _translationHelper.FindBestLanguage(setupLanguage);

                settings.ApplicationSettings.Language = language.Iso2;
            }
        }
    }
}
