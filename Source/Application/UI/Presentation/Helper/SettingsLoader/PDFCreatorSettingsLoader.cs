using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.Globalization;
using System.Linq;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class PDFCreatorSettingsLoader : SettingsLoader
    {
        private readonly IPrinterHelper _printerHelper;
        private readonly EditionHelper _editionHelper;
        private readonly ITranslationHelper _translationHelper;

        public PDFCreatorSettingsLoader(ISettingsMover settingsMover, 
            IInstallationPathProvider installationPathProvider, 
            IDefaultSettingsBuilder defaultSettingsBuilder, 
            IMigrationStorageFactory migrationStorageFactory, 
            IActionOrderChecker actionOrderChecker, 
            ISettingsBackup settingsBackup, 
            ITranslationHelper translationHelper, 
            IPrinterHelper printerHelper, 
            EditionHelper editionHelper,
            ISharedSettingsLoader sharedSettingsLoader,
            IBaseSettingsBuilder baseSettingsBuilder)
            : base(settingsMover, installationPathProvider, defaultSettingsBuilder, migrationStorageFactory, actionOrderChecker, settingsBackup, sharedSettingsLoader, baseSettingsBuilder)
        {
            _printerHelper = printerHelper;
            _editionHelper = editionHelper;
            _translationHelper = translationHelper;
        }

        protected override void ProcessBeforeSaving(PdfCreatorSettings settings) { }

        protected override void ProcessAfterSaving(PdfCreatorSettings settings) { }

        protected override void PrepareForLoading() { }

        protected override void ProcessAfterLoading(PdfCreatorSettings settings)
        {
            _translationHelper.TranslateProfileList(settings.ConversionProfiles);
            CheckLanguage(settings);
            CheckPrinterMappings(settings);
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

        private void CheckPrinterMappings(PdfCreatorSettings settings)
        {
            var printers = _printerHelper.GetPDFCreatorPrinters();

            // if there are no printers, something is broken and we need to fix that first
            if (!printers.Any())
                return;

            //Assign DefaultProfile for all installed printers without mapped profile.
            foreach (var printer in printers)
            {
                if (settings.ApplicationSettings.PrinterMappings.All(o => o.PrinterName != printer))
                    settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping(printer,
                        ProfileGuids.DEFAULT_PROFILE_GUID));
            }
            //Remove uninstalled printers from mapping
            foreach (var mapping in settings.ApplicationSettings.PrinterMappings.ToArray())
            {
                if (printers.All(o => o != mapping.PrinterName))
                    settings.ApplicationSettings.PrinterMappings.Remove(mapping);
            }
            //Check primary printer
            if (
                settings.ApplicationSettings.PrinterMappings.All(
                    o => o.PrinterName != settings.CreatorAppSettings.PrimaryPrinter))
            {
                settings.CreatorAppSettings.PrimaryPrinter =
                    _printerHelper.GetApplicablePDFCreatorPrinter("PDFCreator", "PDFCreator") ?? "";
            }
        }
    }
}
