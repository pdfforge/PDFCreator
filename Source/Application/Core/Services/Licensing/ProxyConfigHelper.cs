using Optional;
using pdfforge.LicenseValidator.Data;
using SystemInterface.IO;

/// <summary>
///  Attention: Any change in this class must also be made in the same class in the "PDFCreatorService" project and in the "PDFCreator Setup" project. We could not currently find a shared project. (The utilities project is not suitable for this, since the hotfolder also accesses it.)
/// </summary>

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public static class ProxyConfigHelper
    {
        public const string ProxyRegPath = "LicenseServerProxy";
        public const string ProxyRegHive = "HKEY_LOCAL_MACHINE";

        private static string ReadRegSetting(string applicationRegistryPath, string subpath, string name, string defaultValue = null)
        {
            try
            {
                var path = PathSafe.Combine(ProxyRegHive, applicationRegistryPath, subpath);
                var value = Microsoft.Win32.Registry.GetValue(path, name, defaultValue)?.ToString();

                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static Option<ProxyCredentials> GetProxyCredentials(string applicationRegistryPath)
        {
            var user = ReadRegSetting(applicationRegistryPath, ProxyRegPath, "Username");
            var password = ReadRegSetting(applicationRegistryPath, ProxyRegPath, "Password");

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
                return Option.None<ProxyCredentials>();

            return new ProxyCredentials(user, password).Some();
        }

        public static Option<ProxyConfig> GetProxyConfig(string applicationRegistryPath)
        {
            var url = ReadRegSetting(applicationRegistryPath, ProxyRegPath, "url");

            if (string.IsNullOrWhiteSpace(url))
                return Option.None<ProxyConfig>();

            return new ProxyConfig(url, GetProxyCredentials(applicationRegistryPath)).Some();
        }
    }
}
