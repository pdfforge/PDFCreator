using Microsoft.Win32;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings
{
    public interface IBaseSettingsBuilder
    {
        PdfCreatorSettings CreateBaseSettings(string primaryPrinter, string defaultLanguage);
    }

    public class DefaultBaseSettingsBuilder : IBaseSettingsBuilder
    {
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;

        public DefaultBaseSettingsBuilder(IDefaultSettingsBuilder defaultSettingsBuilder)
        {
            _defaultSettingsBuilder = defaultSettingsBuilder;
        }

        public PdfCreatorSettings CreateBaseSettings(string primaryPrinter, string defaultLanguage)
        {
            return (PdfCreatorSettings)_defaultSettingsBuilder.CreateDefaultSettings(primaryPrinter, defaultLanguage);
        }
    }

    public class BaseSettingsBuilderWithSharedSettings : IBaseSettingsBuilder
    {
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;
        private readonly IInstallationPathProvider _installationPathProvider;

        public BaseSettingsBuilderWithSharedSettings(IDefaultSettingsBuilder defaultSettingsBuilder, IInstallationPathProvider installationPathProvider)
        {
            _defaultSettingsBuilder = defaultSettingsBuilder;
            _installationPathProvider = installationPathProvider;
        }

        private bool DefaultUserSettingsExist()
        {
            using (var k = Registry.Users.OpenSubKey(@".DEFAULT\" + _installationPathProvider.SettingsRegistryPath))
                return k != null;
        }

        private PdfCreatorSettings LoadDefaultUserSettings(PdfCreatorSettings defaultSettings)
        {
            var defaultUserStorage = new RegistryStorage(RegistryHive.Users,
                @".DEFAULT\" + _installationPathProvider.SettingsRegistryPath);

            var data = Data.CreateDataStorage();

            // Store default settings and then load the machine defaults from HKEY_USERS\.DEFAULT to give them prefrence
            defaultSettings.StoreValues(data, "");
            defaultUserStorage.ReadData("", data);

            // And then load the combined settings with default user overriding our defaults
            var settings = (PdfCreatorSettings)_defaultSettingsBuilder.CreateEmptySettings();
            settings.ReadValues(data);

            return settings;
        }

        public PdfCreatorSettings CreateBaseSettings(string primaryPrinter, string defaultLanguage)
        {
            var defaultSettings = (PdfCreatorSettings)_defaultSettingsBuilder.CreateDefaultSettings(primaryPrinter, defaultLanguage);

            return DefaultUserSettingsExist()
                ? LoadDefaultUserSettings(defaultSettings)
                : defaultSettings;
        }
    }
}
