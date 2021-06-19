using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading
{
    public abstract class SettingsLoader : ISettingsLoader
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISettingsMover _settingsMover;
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;
        private readonly IMigrationStorageFactory _migrationStorageFactory;
        private readonly IActionOrderChecker _actionOrderChecker;
        private readonly ISettingsBackup _settingsBackup;
        private readonly ISharedSettingsLoader _sharedSettingsLoader;
        private readonly IBaseSettingsBuilder _baseSettingsBuilder;

        public SettingsLoader(ISettingsMover settingsMover, IInstallationPathProvider installationPathProvider, IDefaultSettingsBuilder defaultSettingsBuilder, IMigrationStorageFactory migrationStorageFactory, IActionOrderChecker actionOrderChecker, ISettingsBackup settingsBackup, ISharedSettingsLoader sharedSettingsLoader, IBaseSettingsBuilder baseSettingsBuilder)
        {
            _settingsMover = settingsMover;
            _installationPathProvider = installationPathProvider;
            _defaultSettingsBuilder = defaultSettingsBuilder;
            _migrationStorageFactory = migrationStorageFactory;
            _actionOrderChecker = actionOrderChecker;
            _settingsBackup = settingsBackup;
            _sharedSettingsLoader = sharedSettingsLoader;
            _baseSettingsBuilder = baseSettingsBuilder;
        }

        protected abstract void PrepareForLoading();
        protected abstract void ProcessAfterLoading(PdfCreatorSettings settings);
        protected abstract void ProcessBeforeSaving(PdfCreatorSettings settings);
        protected abstract void ProcessAfterSaving(PdfCreatorSettings settings);

        public void SaveSettingsInRegistry(PdfCreatorSettings settings)
        {
            _logger.Debug("Saving settings");
            ProcessBeforeSaving(settings);
            CheckGuids(settings);
            var regStorage = BuildMigrationStorage();
            settings.SaveData(regStorage);
            LogProfiles(settings);
            ProcessAfterSaving(settings);
        }

        public PdfCreatorSettings LoadPdfCreatorSettings()
        {
            MoveSettingsIfRequired();
            PrepareForLoading();
            var settings = (PdfCreatorSettings) _defaultSettingsBuilder.CreateEmptySettings();
            var migrationStorage = BuildMigrationStorage();
            settings.LoadData(migrationStorage);

            _sharedSettingsLoader.ApplySharedSettings(settings);

            if (settings.ConversionProfiles.Count <= 0)
            {
                settings = _baseSettingsBuilder.CreateBaseSettings(FindPrimaryPrinter(), settings.ApplicationSettings.Language);
            }

            CheckAndAddMissingDefaultProfile(settings);
            CheckTitleReplacement(settings);
            CheckDefaultViewers(settings);
            _actionOrderChecker.Check(settings.ConversionProfiles);
            ProcessAfterLoading(settings);
            LogProfiles(settings);

            return settings;
        }

        private void MoveSettingsIfRequired()
        {
            if (!_settingsMover.MoveRequired())
                return;
            _settingsMover.MoveSettings();
        }

        private IStorage BuildMigrationStorage()
        {
            var storage = new RegistryStorage(RegistryHive.CurrentUser, _installationPathProvider.SettingsRegistryPath, true);
            return _migrationStorageFactory.GetMigrationStorage(storage, CreatorAppSettings.ApplicationSettingsVersion, _settingsBackup);
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
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + _installationPathProvider.ApplicationGuid,
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + _installationPathProvider.ApplicationGuid
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
        private void CheckAndAddMissingDefaultProfile(PdfCreatorSettings settings)
        {
            var defaultProfile = settings.GetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);
            if (defaultProfile == null)
            {
                defaultProfile = _defaultSettingsBuilder.CreateDefaultProfile();
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
    }
}
