using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface ISignaturePasswordCheck
    {
        bool IsValidPassword(string certificateFile, string certificatePassword);
    }

    public class SignaturePasswordCheckCached : ISignaturePasswordCheck
    {
        private class SignatureCheckResult
        {
            public SignatureCheckResult(string signatureHash, bool isValid)
            {
                SignatureHash = signatureHash;
                IsValid = isValid;
            }

            public string SignatureHash { get; }
            public bool IsValid { get; }
        }

        private readonly IHashUtil _hashUtil;
        private readonly IFile _file;
        private readonly List<SignatureCheckResult> _signatureCache;

        public SignaturePasswordCheckCached(IHashUtil hashUtil, IFile file)
        {
            _hashUtil = hashUtil;
            _file = file;
            _signatureCache = new List<SignatureCheckResult>();
        }

        /// <summary>
        ///     This method returns true if a certificatePassword for a given certificate file is valid.
        /// </summary>
        /// <param name="certificateFile">Name of p12 or pfx certificate file.</param>
        /// <param name="certificatePassword">A certificatePassword string.</param>
        /// <returns>
        ///     True if the certificatePassword is valid.
        /// </returns>
        public bool IsValidPassword(string certificateFile, string certificatePassword)
        {
            if (string.IsNullOrWhiteSpace(certificateFile))
                return false;

            var lastModificationDate = _file.GetLastWriteTimeUtc(certificateFile);
            var signatureHash = _hashUtil.GetSha1Hash(certificateFile + "|" + certificatePassword + "|" + lastModificationDate);
            var lastSignatureCheck = _signatureCache.FirstOrDefault(r => r.SignatureHash == signatureHash);
            if (lastSignatureCheck != null)
                return lastSignatureCheck.IsValid;

            try
            {
                var cert = new X509Certificate2(certificateFile, certificatePassword);
                _signatureCache.Add(new SignatureCheckResult(signatureHash, true));
                return true;
            }
            catch //(CryptographicException)
            {
                _signatureCache.Add(new SignatureCheckResult(signatureHash, false));
                return false;
            }
        }
    }
}
