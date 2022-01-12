using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Utilities.Registry;
using System.Security;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsMover
    {
        bool MoveRequired();

        bool MoveSettings();
    }

    /// <summary>
    ///     It is good practice to store Registry settings under HKEY_CURRENT_USER\Software\CompanyName\ProductName
    ///     In the past, we stored them under HKEY_CURRENT_USER\Software\PDFCreator.net
    ///     They are now stored under HKEY_CURRENT_USER\Software\pdfforge\PDFCreator
    ///     This class checks if old settings exist, if they need to be moved to the new location
    ///     and performs the move if required.
    /// </summary>
    public class SettingsMover : ISettingsMover
    {
        private const string OldRegistryPath = @"Software\PDFCreator.Net";
        private readonly string _newRegistryPath;
        private readonly IRegistryUtility _registryUtility;
        private readonly IRegistry _registryWrap;

        public SettingsMover(IRegistry registryWrap, IRegistryUtility registryUtility, IInstallationPathProvider installationPathProvider)
        {
            _registryWrap = registryWrap;
            _registryUtility = registryUtility;
            _newRegistryPath = installationPathProvider.ApplicationRegistryPath;
        }

        public bool MoveRequired()
        {
            try
            {
                var regKey = _registryWrap.CurrentUser.OpenSubKey(OldRegistryPath);
                if (regKey == null)
                    return false;

                regKey.Close();

                regKey = _registryWrap.CurrentUser.OpenSubKey(_newRegistryPath);
                if (regKey == null)
                    return true;

                regKey.Close();
            }
            catch (SecurityException)
            {
            }

            return false;
        }

        public bool MoveSettings()
        {
            if (!MoveRequired())
                return false;

            return _registryUtility.RenameSubKey(_registryWrap.CurrentUser, OldRegistryPath, _newRegistryPath);
        }
    }
}
