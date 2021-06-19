using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading
{
    public interface ISharedSettingsLoader
    {
        void ApplySharedSettings(PdfCreatorSettings currentSettings);

        IEnumerable<PrinterMapping> GetSharedPrinterMappings();
    }

    public class SharedSettingsLoader : ISharedSettingsLoader
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IIniSettingsLoader _iniSettingsLoader;
        private readonly IDirectory _directory;
        private readonly IGpoSettings _gpoSettings;
        private readonly IProgramDataDirectoryHelper _programDataDirectoryHelper;

        public SharedSettingsLoader(IIniSettingsLoader iniSettingsLoader, IDirectory directory,
            IGpoSettings gpoSettings, IProgramDataDirectoryHelper programDataDirectoryHelper)
        {
            _iniSettingsLoader = iniSettingsLoader;
            _directory = directory;
            _gpoSettings = gpoSettings;
            _programDataDirectoryHelper = programDataDirectoryHelper;
        }

        public void ApplySharedSettings(PdfCreatorSettings currentSettings)
        {
            if (!_gpoSettings.LoadSharedAppSettings && !_gpoSettings.LoadSharedProfiles)
                return;

            _logger.Info("Apply shared settings.");
            var sharedSettings = GetSharedSettings();
            if (sharedSettings == null)
                return;

            ApplyAppSettings(currentSettings, sharedSettings);
            ApplyProfiles(currentSettings, sharedSettings);
        }

        private string GetSharedSettingsIniFile()
        {
            try
            {
                var dir = _programDataDirectoryHelper.GetDir();
                var files = _directory.GetFiles(dir, "*.ini");
                if (files.Length > 0)
                    return files[0];
            }
            catch { }
            return null;
        }

        private void ApplyAppSettings(PdfCreatorSettings currentSettings, PdfCreatorSettings sharedSettings)
        {
            if (!_gpoSettings.LoadSharedAppSettings)
                return;

            _logger.Info("Apply shared app settings.");
            currentSettings.ApplicationSettings = sharedSettings.ApplicationSettings;
        }

        private void ApplyProfiles(PdfCreatorSettings currentSettings, PdfCreatorSettings sharedSettings)
        {
            if (!_gpoSettings.LoadSharedProfiles)
                return;

            _logger.Info("Apply shared profiles.");
            if (_gpoSettings.AllowUserDefinedProfiles)
            {
                var additionalProfiles = new List<ConversionProfile>();

                foreach (var currentProfile in currentSettings.ConversionProfiles)
                {
                    //do not add current profiles which were previously shared
                    if (!currentProfile.Properties.IsShared && !ProfileExists(currentProfile, sharedSettings.ConversionProfiles))
                        additionalProfiles.Add(currentProfile);
                }
                foreach (var profile in additionalProfiles)
                    sharedSettings.ConversionProfiles.Add(profile);
            }

            currentSettings.ConversionProfiles = sharedSettings.ConversionProfiles;
        }

        private bool ProfileExists(ConversionProfile profile, IList<ConversionProfile> profiles)
        {
            return profiles.Any(p => p.Name == profile.Name || p.Guid == profile.Guid);
        }

        public IEnumerable<PrinterMapping> GetSharedPrinterMappings()
        {
            if (_gpoSettings.LoadSharedAppSettings)
            {
                var sharedSettings = GetSharedSettings();
                if (sharedSettings != null)
                    return sharedSettings.ApplicationSettings.PrinterMappings;
            }

            return new List<PrinterMapping>();
        }

        private PdfCreatorSettings GetSharedSettings()
        {
            var iniFile = GetSharedSettingsIniFile();
            if (iniFile == null)
            {
                _logger.Debug("Could not find shared settings.ini.");
                return null;
            }

            try
            {
                _logger.Info("Get shared settings from '" + iniFile + "'.");

                var sharedSettings = (PdfCreatorSettings)_iniSettingsLoader.LoadIniSettings(iniFile);
                foreach (var profile in sharedSettings.ConversionProfiles)
                    profile.Properties.IsShared = true;

                return sharedSettings;
            }
            catch
            {
                _logger.Warn("Could not load settings from '" + iniFile + "'.");
                return null;
            }
        }
    }

    public class FreeSharedSettingsLoader : ISharedSettingsLoader
    {
        public void ApplySharedSettings(PdfCreatorSettings currentSettings)
        {
        }

        public IEnumerable<PrinterMapping> GetSharedPrinterMappings()
        {
            return new List<PrinterMapping>();
        }
    }
}
