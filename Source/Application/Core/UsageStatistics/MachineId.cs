using System;
using System.Security.Cryptography;
using System.Text;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    internal class MachineId
    {
        private long SystemVolumeSerial { get; }
        private string WindowsProductId { get; }

        public MachineId(long systemVolumeSerial, string windowsProductId)
        {
            SystemVolumeSerial = systemVolumeSerial;
            WindowsProductId = windowsProductId.Replace("-", "");
        }

        public string CalculateMachineHash()
        {
            const string salt = "GQ461qpa6s0SeD4qabZce6JVP7sTywtN";
            return CalculateMachineHash(salt);
        }

        private string CalculateMachineHash(string salt)
        {
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentException("salt");

            var serialString = SystemVolumeSerial.ToString("X8");
            var hashBase = serialString + WindowsProductId + salt;
            return GetSha1Hash(hashBase);
        }

        public static string GetSha1Hash(string text)
        {
            var result = "";
            var sha1 = new SHA1CryptoServiceProvider();
            var arrayData = Encoding.ASCII.GetBytes(text);
            var arrayResult = sha1.ComputeHash(arrayData);

            foreach (var entry in arrayResult)
            {
                var temp = Convert.ToString(entry, 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                result += temp;
            }

            return result;
        }
    }
}
