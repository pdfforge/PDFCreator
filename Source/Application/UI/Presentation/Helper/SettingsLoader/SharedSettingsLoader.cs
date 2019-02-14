using NLog;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface ISharedSettingsLoader
    {
        void ApplySharedSettings(PdfCreatorSettings currentSettings);
    }

    public class SharedSettingsLoader : ISharedSettingsLoader
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private const string SharedSettingsFolder = @"%ProgramData%\pdfforge\PDFCreator";
        private readonly IIniSettingsLoader _iniSettingsLoader;
        private readonly IDirectory _directory;
        private readonly IGpoSettings _gpoSettings;

        public SharedSettingsLoader(IIniSettingsLoader iniSettingsLoader, IDirectory directory, IGpoSettings gpoSettings)
        {
            _iniSettingsLoader = iniSettingsLoader;
            _directory = directory;
            _gpoSettings = gpoSettings;
        }

        public void ApplySharedSettings(PdfCreatorSettings currentSettings)
        {
            if (!_gpoSettings.LoadSharedAppSettings && !_gpoSettings.LoadSharedProfiles)
                return;

            var iniFile = GetSharedSettingsIniFile();
            if (iniFile == null)
                return;

            try
            {
                var sharedSettings = (PdfCreatorSettings)_iniSettingsLoader.LoadIniSettings(iniFile);
                foreach (var profile in sharedSettings.ConversionProfiles)
                    profile.Properties.IsShared = true;

                ApplyAppSettings(currentSettings, sharedSettings);
                ApplyProfiles(currentSettings, sharedSettings);

                _logger.Info("Apply shared settings from '" + iniFile + "'.");
            }
            catch
            {
                _logger.Warn("Could not load settings from '" + iniFile + "'.");
            }
        }

        private string GetSharedSettingsIniFile()
        {
            try
            {
                var dir = Environment.ExpandEnvironmentVariables(SharedSettingsFolder);
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

            currentSettings.ApplicationSettings = sharedSettings.ApplicationSettings;
        }

        private void ApplyProfiles(PdfCreatorSettings currentSettings, PdfCreatorSettings sharedSettings)
        {
            if (!_gpoSettings.LoadSharedProfiles)
                return;

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
    }
}
