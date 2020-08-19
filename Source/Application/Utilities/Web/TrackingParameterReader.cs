using Microsoft.Win32;
using System.Security;

namespace pdfforge.PDFCreator.Utilities.Web
{
    public static class TrackingParameterReader
    {
        public static TrackingParameters ReadFromRegistry()
        {
            var regKey = @"SOFTWARE\pdfforge\PDFCreator\Parameters";

            var parameters = new TrackingParameters("", "", "", "");

            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regKey, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (key != null)
                    {
                        parameters = new TrackingParameters(
                            key.GetValue("cmp", "") as string,
                            key.GetValue("key1", "") as string,
                            key.GetValue("key2", "") as string,
                            key.GetValue("keyb", "") as string);
                    }
                }
            }
            catch (SecurityException)
            {
                // ignore access problems
            }

            return parameters;
        }
    }
}
