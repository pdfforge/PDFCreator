using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using NLog;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Path = System.IO.Path;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextSigner
    {
        //ActionId = 12;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Add a signature (set in profile) to a document, that is opened in the stamper.
        ///     The function does nothing, if signature settings are disabled.
        /// </summary>
        /// <param name="signer">Signer with document</param>
        /// <param name="profile">Profile with signature settings</param>
        /// <param name="jobPasswords">Passwords with PdfSignaturePassword</param>
        /// <param name="accounts">List of accounts</param>
        /// <exception cref="ProcessingException">In case of any error.</exception>
        public void SignPdfFile(PdfSigner signer, ConversionProfile profile, JobPasswords jobPasswords, Accounts accounts)
        {
            var signing = profile.PdfSettings.Signature;

            if (!profile.PdfSettings.Signature.Enabled) //Leave without signing
                return;

            _logger.Debug("Start signing file.");

            if (!File.Exists(signing.CertificateFile))
            {
                _logger.Error("Unable to find certification file: " + signing.CertificateFile);
                throw new ProcessingException("Canceled signing. Unable to find certification file.", ErrorCode.Signature_FileNotFound);
            }

            signing.CertificateFile = Path.GetFullPath(signing.CertificateFile);

            if (string.IsNullOrEmpty(jobPasswords.PdfSignaturePassword))
            {
                _logger.Error("Launched signing without certification password.");
                throw new ProcessingException("Launched signing without certification password.", ErrorCode.Signature_LaunchedSigningWithoutPassword);
            }
            if (IsValidCertificatePassword(signing.CertificateFile, jobPasswords.PdfSignaturePassword) == false)
            {
                _logger.Error("Canceled signing. The password for certificate '" + signing.CertificateFile + "' is wrong.");
                throw new ProcessingException("Canceled signing. The password for certificate '" + signing.CertificateFile + "' is wrong.", ErrorCode.Signature_WrongCertificatePassword);
            }
            if (CertificateHasPrivateKey(signing.CertificateFile, jobPasswords.PdfSignaturePassword) == false)
            {
                _logger.Error("Canceled signing. The certificate '" + signing.CertificateFile + "' has no private key.");
                throw new ProcessingException(
                    "Canceled signing. The certificate '" + signing.CertificateFile + "' has no private key.", ErrorCode.Signature_NoPrivateKey);
            }

            var timeServerAccount = accounts.GetTimeServerAccount(profile);
            if (timeServerAccount == null)
            {
                _logger.Error("Launched signing without available timeserver account.");
                throw new ProcessingException("Launched signing without available timeserver account.", ErrorCode.Signature_NoTimeServerAccount);
            }

            try
            {
                DoSignPdfFile(signer, signing, jobPasswords, timeServerAccount);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (PdfException ex) when (ex.InnerException is WebException)
            {
                throw new ProcessingException(ex.GetType() + " while signing:" + Environment.NewLine + ex.Message, ErrorCode.Signature_NoTimeServerConnection, ex);
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " while signing:" + Environment.NewLine + ex.Message, ErrorCode.Signature_GenericError, ex);
            }
        }

        private string GetCertificateAlias(Pkcs12Store store)
        {
            foreach (string al in store.Aliases)
            {
                if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate)
                {
                    return al;
                }
            }

            throw new CryptographicException("Could not find a private key in the certificate");
        }

        private ICipherParameters GetPrivateKey(Pkcs12Store store, string alias)
        {
            return store.GetKey(alias).Key;
        }

        private IList<X509Certificate> GetCertificateChain(Pkcs12Store store, string alias)
        {
            return store.GetCertificateChain(alias)
                .Select(x => x.Certificate)
                .ToList();
        }

        private Pkcs12Store GetCertificateStore(string certificateFile, string password)
        {
            using (var fsCert = new FileStream(certificateFile, FileMode.Open, FileAccess.Read))
            {
                return new Pkcs12Store(fsCert, password.ToCharArray());
            }
        }

        private ITSAClient BuildTimeServerClient(TimeServerAccount timeServerAccount)
        {
            if (string.IsNullOrWhiteSpace(timeServerAccount.Url))
                return null;

            return timeServerAccount.IsSecured
                ? new TSAClientBouncyCastle(timeServerAccount.Url, timeServerAccount.UserName, timeServerAccount.Password)
                : new TSAClientBouncyCastle(timeServerAccount.Url);
        }

        private IOcspClient BuildOcspClient()
        {
            var verifier = new OCSPVerifier(null, null);
            return new OcspClientBouncyCastle(verifier);
        }

        private void BuildSignatureAppearance(PdfSigner signer, Signature signing)
        {
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetReason(signing.SignReason);
            appearance.SetContact(signing.SignContact);
            appearance.SetLocation(signing.SignLocation);

            var arial = PdfFontFactory.CreateFont(Environment.GetEnvironmentVariable("WINDIR") + "\\Fonts\\Arial.ttf", PdfName.WinAnsiEncoding.GetValue(), true);

            appearance.SetLayer2Font(arial);
            if (!signing.AllowMultiSigning)
            {
                signer.SetCertificationLevel(PdfSigner.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS);
                appearance.SetCertificate(signer.GetSignatureAppearance().GetCertificate());
            }

            if (signing.DisplaySignatureInDocument)
            {
                var signPage = SignPageNr(signer, signing);
                var left = signing.LeftX;
                var bottom = signing.LeftY;
                var width = signing.RightX - left;
                var height = signing.RightY - bottom;

                var rect = new Rectangle(left, bottom, width, height);
                appearance.SetPageRect(rect);
                appearance.SetPageNumber(signPage);
            }
        }

        private void DoSignPdfFile(PdfSigner signer, Signature signing, JobPasswords jobPasswords, TimeServerAccount timeServerAccount)
        {
            Pkcs12Store store = GetCertificateStore(signing.CertificateFile, jobPasswords.PdfSignaturePassword);
            var certificateAlias = GetCertificateAlias(store);
            var pk = GetPrivateKey(store, certificateAlias);

            BuildSignatureAppearance(signer, signing);

            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA512);
            var chain = GetCertificateChain(store, certificateAlias).ToArray();
            var ocspClient = BuildOcspClient();
            var tsaClient = BuildTimeServerClient(timeServerAccount);

            var cryptoStandard = PdfSigner.CryptoStandard.CADES;
            signer.SignDetached(pks, chain, null, ocspClient, tsaClient, 0, cryptoStandard);
        }

        private bool IsValidCertificatePassword(string certificateFilename, string certificatePassword)
        {
            try
            {
                var cert = new X509Certificate2(certificateFilename, certificatePassword);
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private bool CertificateHasPrivateKey(string certificateFilename, string certificatePassword)
        {
            var cert = new X509Certificate2(certificateFilename, certificatePassword);
            if (cert.HasPrivateKey)
                return true;
            return false;
        }

        private int SignPageNr(PdfSigner signer, Signature signing)
        {
            switch (signing.SignaturePage)
            {
                case SignaturePage.CustomPage:
                    if (signing.SignatureCustomPage > signer.GetDocument().GetNumberOfPages())
                        return signer.GetDocument().GetNumberOfPages();
                    if (signing.SignatureCustomPage < 1)
                        return 1;
                    return signing.SignatureCustomPage;

                case SignaturePage.LastPage:
                    return signer.GetDocument().GetNumberOfPages();

                default:
                    return 1;
            }
        }
    }
}
