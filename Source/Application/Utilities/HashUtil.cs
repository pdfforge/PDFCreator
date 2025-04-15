using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IHashUtil
    {
        string GetSha1Hash(string text);

        string CalculateFileMd5(string filepath);

        bool VerifyFileMd5(string filepath, string expectedMd5);
        public string GetSha256Hash(string toHash);
    }

    public class HashUtil : IHashUtil
    {
        /// <summary>
        ///     Gets the SHA1 hash.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>hashed text</returns>
        public string GetSha1Hash(string text)
        {
            var hashAlgorithm = SHA1.Create();

            string result = null;

            var arrayData = Encoding.ASCII.GetBytes(text);
            var arrayResult = hashAlgorithm.ComputeHash(arrayData);

            for (var i = 0; i < arrayResult.Length; i++)
            {
                var temp = Convert.ToString(arrayResult[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                result += temp;
            }

            return result;
        }

        public string CalculateFileMd5(string filepath)
        {
            var fileCheck = File.OpenRead(filepath);

            // calculate MD5-Hash from Byte-Array
            var hashAlgorithm = MD5.Create();
            var md5Hash = hashAlgorithm.ComputeHash(fileCheck);
            fileCheck.Close();

            var md5 = BitConverter.ToString(md5Hash).Replace("-", "").ToLowerInvariant();
            return md5;
        }

        public bool VerifyFileMd5(string filepath, string expectedMd5)
        {
            var md5 = CalculateFileMd5(filepath);
            return md5 == expectedMd5.ToLowerInvariant();
        }

        public string GetSha256Hash(string toHash)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
