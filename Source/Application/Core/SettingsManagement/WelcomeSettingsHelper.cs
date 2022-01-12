using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IWelcomeSettingsHelper
    {
        bool CheckIfRequiredAndSetCurrentVersion();
    }

    public class WelcomeSettingsHelper : IWelcomeSettingsHelper
    {
        private readonly string _registryKeyForWelcomeSettings;
        public const string RegistryValueForWelcomeVersion = @"LatestWelcomeVersion";

        private readonly IRegistry _registryWrap;
        private readonly IVersionHelper _versionHelper;

        public WelcomeSettingsHelper(IRegistry registryWrap, IVersionHelper versionHelper, IInstallationPathProvider installationPathProvider)
        {
            _registryWrap = registryWrap;
            _versionHelper = versionHelper;
            _registryKeyForWelcomeSettings = PathSafe.Combine("HKEY_CURRENT_USER", installationPathProvider.ApplicationRegistryPath);
        }

        public bool CheckIfRequiredAndSetCurrentVersion()
        {
            var currentApplicationVersion = _versionHelper.FormatWithBuildNumber();
            var welcomeVersionFromRegistry = GetWelcomeVersionFromRegistry();

            if (currentApplicationVersion.Equals(welcomeVersionFromRegistry, StringComparison.OrdinalIgnoreCase))
                return false;

            SetCurrentApplicationVersionAsWelcomeVersionInRegistry();
            return true;
        }

        public void SetCurrentApplicationVersionAsWelcomeVersionInRegistry()
        {
            var currentApplicationVersion = _versionHelper.FormatWithBuildNumber();
            _registryWrap.SetValue(_registryKeyForWelcomeSettings, RegistryValueForWelcomeVersion, currentApplicationVersion);
        }

        private string GetWelcomeVersionFromRegistry()
        {
            var value = _registryWrap.GetValue(_registryKeyForWelcomeSettings, RegistryValueForWelcomeVersion, null);
            if (value == null)
                return "";
            return value.ToString();
        }
    }
}
