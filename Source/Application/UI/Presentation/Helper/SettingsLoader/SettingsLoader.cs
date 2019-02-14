using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface ISettingsLoader
    {
        PdfCreatorSettings LoadPdfCreatorSettings();

        void SaveSettingsInRegistry(PdfCreatorSettings settings);
    }

    public abstract class SettingsLoaderBase : ISettingsLoader
    {
        protected readonly IInstallationPathProvider InstallationPathProvider;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPrinterHelper _printerHelper;
        private readonly EditionHelper _editionHelper;
        protected readonly IDefaultSettingsBuilder DefaultSettingsBuilder;
        private readonly IMigrationStorageFactory _migrationStorageFactory;
        private readonly ITranslationHelper _translationHelper;
        private readonly ISettingsMover _settingsMover;

        public SettingsLoaderBase(ITranslationHelper translationHelper, ISettingsMover settingsMover, IInstallationPathProvider installationPathProvider, IPrinterHelper printerHelper, EditionHelper editionHelper, IDefaultSettingsBuilder defaultSettingsBuilder, IMigrationStorageFactory migrationStorageFactory)
        {
            _settingsMover = settingsMover;
            InstallationPathProvider = installationPathProvider;
            _printerHelper = printerHelper;
            _editionHelper = editionHelper;
            DefaultSettingsBuilder = defaultSettingsBuilder;
            _migrationStorageFactory = migrationStorageFactory;
            _translationHelper = translationHelper;
        }

        public void SaveSettingsInRegistry(PdfCreatorSettings settings)
        {
            CheckGuids(settings);
            var regStorage = BuildStorage();
            _logger.Debug("Saving settings");
            settings.SaveData(regStorage);
            LogProfiles(settings);
        }

        public PdfCreatorSettings LoadPdfCreatorSettings()
        {
            MoveSettingsIfRequired();
            var storage = _migrationStorageFactory.GetMigrationStorage(BuildStorage(), CreatorAppSettings.ApplicationSettingsVersion);

            var settings = (PdfCreatorSettings)DefaultSettingsBuilder.CreateEmptySettings();

            if (UserSettingsExist())
            {
                var settingsMigrationStorage = _migrationStorageFactory.GetMigrationStorage(storage, CreatorAppSettings.ApplicationSettingsVersion);
                settings.LoadData(settingsMigrationStorage);
            }

            ApplySharedSettings(settings);

            if (!_translationHelper.HasTranslation(settings.ApplicationSettings.Language))
            {
                var language = _translationHelper.FindBestLanguage(CultureInfo.CurrentCulture);
                settings.ApplicationSettings.Language = language.Iso2;
            }

            if (!CheckValidSettings(settings))
            {
                settings = CreateDefaultSettings(FindPrimaryPrinter(), settings.ApplicationSettings.Language);
            }

            CheckAndAddMissingDefaultProfile(settings, DefaultSettingsBuilder);
            CheckPrinterMappings(settings);
            CheckTitleReplacement(settings);
            CheckUpdateInterval(settings);
            CheckDefaultViewers(settings);

            _translationHelper.TranslateProfileList(settings.ConversionProfiles);

            LogProfiles(settings);

            return settings;
        }

        protected virtual void ApplySharedSettings(PdfCreatorSettings settings)
        { }

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

        protected abstract PdfCreatorSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage);

        private void LogProfiles(PdfCreatorSettings settings)
        {
            if (!_logger.IsTraceEnabled)
                return;

            _logger.Trace("Profiles:");
            foreach (var conversionProfile in settings.ConversionProfiles)
            {
                _logger.Trace(conversionProfile.Name);
            }
        }

        private void MoveSettingsIfRequired()
        {
            if (!_settingsMover.MoveRequired())
                return;
            _settingsMover.MoveSettings();
        }

        private IStorage BuildStorage()
        {
            var storage = new RegistryStorage(RegistryHive.CurrentUser, InstallationPathProvider.SettingsRegistryPath, true);

            return storage;
        }

        private bool UserSettingsExist()
        {
            using (var k = Registry.CurrentUser.OpenSubKey(InstallationPathProvider.SettingsRegistryPath))
                return k != null;
        }

        public bool CheckValidSettings(PdfCreatorSettings settings)
        {
            return settings.ConversionProfiles.Count > 0;
        }

        /// <summary>
        ///     Finds the primary printer by checking the printer setting from the setup
        /// </summary>
        /// <returns>
        ///     The name of the printer that was defined in the setup. If it is empty or does not exist, the return value is
        ///     "PDFCreator"
        /// </returns>
        private string FindPrimaryPrinter()
        {
            var regKeys = new List<string>
            {
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + InstallationPathProvider.ApplicationGuid,
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + InstallationPathProvider.ApplicationGuid
            };

            string printer = null;

            foreach (var regKey in regKeys)
            {
                if (printer == null)
                {
                    var o = Registry.GetValue(regKey, "Printername", null);
                    if (o != null)
                    {
                        printer = o.ToString();
                        if (!string.IsNullOrEmpty(printer))
                            return printer;
                    }
                }
            }

            return "PDFCreator";
        }

        /// <summary>
        ///     Functions checks, if a default profile exists and adds one.
        /// </summary>
        private void CheckAndAddMissingDefaultProfile(PdfCreatorSettings settings, IDefaultSettingsBuilder settingsBuilder)
        {
            var defaultProfile = settings.GetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);
            if (defaultProfile == null)
            {
                defaultProfile = settingsBuilder.CreateDefaultProfile();
                settings.ConversionProfiles.Add(defaultProfile);
            }
            else
            {
                defaultProfile.Properties.Deletable = false;
            }
        }

        /// <summary>
        ///     Sets new random GUID for profiles if the GUID is empty or exists twice
        /// </summary>
        private void CheckGuids(PdfCreatorSettings settings)
        {
            var guidList = new List<string>();
            foreach (var profile in settings.ConversionProfiles)
            {
                if (string.IsNullOrWhiteSpace(profile.Guid)
                    || guidList.Contains(profile.Guid))
                {
                    profile.Guid = Guid.NewGuid().ToString();
                }
                guidList.Add(profile.Guid);
            }
        }

        private void CheckTitleReplacement(PdfCreatorSettings settings)
        {
            var titleReplacements = settings.ApplicationSettings.TitleReplacement.ToList();

            titleReplacements.RemoveAll(x => !x.IsValid());
            titleReplacements.Sort((a, b) => string.Compare(b.Search, a.Search, StringComparison.InvariantCultureIgnoreCase));

            settings.ApplicationSettings.TitleReplacement = new ObservableCollection<TitleReplacement>(titleReplacements);
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

        private void CheckDefaultViewers(PdfCreatorSettings settings)
        {
            foreach (var outputFormat in PdfCreatorSettings.GetDefaultViewerFormats())
            {
                if (!settings.DefaultViewers.Any(v => v.OutputFormat == outputFormat))
                {
                    settings.DefaultViewers.Add(new DefaultViewer
                    {
                        IsActive = false,
                        OutputFormat = outputFormat,
                        Parameters = "",
                        Path = ""
                    });
                }
            }
        }
    }

    public class SettingsLoader : SettingsLoaderBase
    {
        public SettingsLoader(
            ITranslationHelper translationHelper,
            ISettingsMover settingsMover,
            IInstallationPathProvider installationPathProvider,
            IPrinterHelper printerHelper,
            EditionHelper editionHelper,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IMigrationStorageFactory migrationStorageFactory) :
            base(translationHelper, settingsMover, installationPathProvider, printerHelper, editionHelper, defaultSettingsBuilder, migrationStorageFactory)
        {
        }

        protected override PdfCreatorSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage)
        {
            return (PdfCreatorSettings)DefaultSettingsBuilder.CreateDefaultSettings(primaryPrinter, defaultLanguage);
        }
    }
}
