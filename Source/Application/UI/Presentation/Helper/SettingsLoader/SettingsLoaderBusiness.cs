using Microsoft.Win32;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class SettingsLoaderBusiness : SettingsLoaderBase
    {
        private readonly ISharedSettingsLoader _sharedSettingsLoader;

        public SettingsLoaderBusiness(
            ITranslationHelper translationHelper,
            ISettingsMover settingsMover,
            IInstallationPathProvider installationPathProvider,
            IPrinterHelper printerHelper,
            EditionHelper editionHelper,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IMigrationStorageFactory migrationStorageFactory,
            ISharedSettingsLoader sharedSettingsLoader
            ) :
            base(translationHelper, settingsMover, installationPathProvider, printerHelper, editionHelper, defaultSettingsBuilder, migrationStorageFactory)
        {
            _sharedSettingsLoader = sharedSettingsLoader;
        }

        private bool DefaultUserSettingsExist()
        {
            using (var k = Registry.Users.OpenSubKey(@".DEFAULT\" + InstallationPathProvider.SettingsRegistryPath))
                return k != null;
        }

        private PdfCreatorSettings LoadDefaultUserSettings(PdfCreatorSettings defaultSettings)
        {
            var defaultUserStorage = new RegistryStorage(RegistryHive.Users,
                @".DEFAULT\" + InstallationPathProvider.SettingsRegistryPath);

            var data = Data.CreateDataStorage();

            // Store default settings and then load the machine defaults from HKEY_USERS\.DEFAULT to give them prefrence
            defaultSettings.StoreValues(data, "");
            defaultUserStorage.ReadData("", data);

            // And then load the combined settings with default user overriding our defaults
            var settings = (PdfCreatorSettings)DefaultSettingsBuilder.CreateEmptySettings();
            settings.ReadValues(data);

            return settings;
        }

        protected override PdfCreatorSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage)
        {
            var defaultSettings = (PdfCreatorSettings)DefaultSettingsBuilder.CreateDefaultSettings(primaryPrinter, defaultLanguage);

            return DefaultUserSettingsExist()
                ? LoadDefaultUserSettings(defaultSettings)
                : defaultSettings;
        }

        protected override void ApplySharedSettings(PdfCreatorSettings settings)
        {
            _sharedSettingsLoader.ApplySharedSettings(settings);
        }
    }
}
