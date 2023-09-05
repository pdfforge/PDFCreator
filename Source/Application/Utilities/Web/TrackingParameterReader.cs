using Microsoft.Win32;
using System.Security;

namespace pdfforge.PDFCreator.Utilities.Web
{
    public static class TrackingParameterReader
    {
        public static TrackingParameters ReadFromRegistry(string pdfcreatorRegistryKey)
        {
            var campaign = "";
            var key1 = "";
            var key2 = "";
            var keyb = "";
            var licenseKey = "";

            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(pdfcreatorRegistryKey, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (key != null)
                    {
                        using (var parametersSubKey = key.OpenSubKey("Parameters", RegistryKeyPermissionCheck.ReadSubTree))
                        {
                            if (parametersSubKey != null)
                            {
                                campaign = parametersSubKey.GetValue("cmp", "") as string;
                                key1 = parametersSubKey.GetValue("key1", "") as string;
                                key2 = parametersSubKey.GetValue("key2", "") as string;
                                keyb = parametersSubKey.GetValue("keyb", "") as string;
                            }
                        }

                        licenseKey = key.GetValue(@"License", "") as string;
                    }
                }
            }
            catch (SecurityException)
            {
                // ignore access problems
            }

            return new TrackingParameters(campaign, key1, key2, keyb, licenseKey);
        }
    }
}
