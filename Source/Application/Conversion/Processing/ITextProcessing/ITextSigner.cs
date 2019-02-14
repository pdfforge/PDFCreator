using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
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
        /// <param name="stamper">Stamper with document</param>
        /// <param name="profile">Profile with signature settings</param>
        /// <param name="jobPasswords">Passwords with PdfSignaturePassword</param>
        /// <param name="accounts">List of accounts</param>
        /// <exception cref="ProcessingException">In case of any error.</exception>
        public void SignPdfFile(PdfStamper stamper, ConversionProfile profile, JobPasswords jobPasswords, Accounts accounts)
        {
            var signing = profile.PdfSettings.Signature;

            if (!profile.PdfSettings.Signature.Enabled) //Leave without signing
                return;

            _logger.Debug("Start signing file.");

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
                DoSignPdfFile(stamper, signing, jobPasswords, timeServerAccount);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (WebException ex)
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
            var verifier = new OcspVerifier(null, null);
            return new OcspClientBouncyCastle(verifier);
        }

        private PdfSignatureAppearance BuildSignatureAppearance(PdfStamper stamper, Signature signing)
        {
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = signing.SignReason;
            appearance.Contact = signing.SignContact;
            appearance.Location = signing.SignLocation;
            var arial = BaseFont.CreateFont(Environment.GetEnvironmentVariable("WINDIR") + "\\Fonts\\Arial.ttf", BaseFont.CP1252, true);
            appearance.Layer2Font = new Font(arial, 12);
            if (!signing.AllowMultiSigning)
                //Lock PDF, except for form filling (irrelevant for PDFCreator)
                appearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS;

            if (signing.DisplaySignatureInDocument)
            {
                var signPage = SignPageNr(stamper, signing);

                appearance.SetVisibleSignature(new Rectangle(signing.LeftX, signing.LeftY, signing.RightX, signing.RightY),
                    signPage, null);
            }

            return appearance;
        }

        private void DoSignPdfFile(PdfStamper stamper, Signature signing, JobPasswords jobPasswords, TimeServerAccount timeServerAccount)
        {
            Pkcs12Store store = GetCertificateStore(signing.CertificateFile, jobPasswords.PdfSignaturePassword);
            var certificateAlias = GetCertificateAlias(store);
            var pk = GetPrivateKey(store, certificateAlias);

            var appearance = BuildSignatureAppearance(stamper, signing);

            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA512);
            var chain = GetCertificateChain(store, certificateAlias);
            var ocspClient = BuildOcspClient();
            var tsaClient = BuildTimeServerClient(timeServerAccount);

            var cryptoStandard = CryptoStandard.CADES;
            MakeSignature.SignDetached(appearance, pks, chain, null, ocspClient, tsaClient, 0, cryptoStandard);
        }

        private bool IsValidCertificatePassword(string certficateFilename, string certifcatePassword)
        {
            try
            {
                var cert = new X509Certificate2(certficateFilename, certifcatePassword);
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private bool CertificateHasPrivateKey(string certficateFilename, string certifcatePassword)
        {
            var cert = new X509Certificate2(certficateFilename, certifcatePassword);
            if (cert.HasPrivateKey)
                return true;
            return false;
        }

        private int SignPageNr(PdfStamper stamper, Signature signing)
        {
            switch (signing.SignaturePage)
            {
                case SignaturePage.CustomPage:
                    if (signing.SignatureCustomPage > stamper.Reader.NumberOfPages)
                        return stamper.Reader.NumberOfPages;
                    if (signing.SignatureCustomPage < 1)
                        return 1;
                    return signing.SignatureCustomPage;

                case SignaturePage.LastPage:
                    return stamper.Reader.NumberOfPages;
                //case SignaturePosition.FirstPage:
                default:
                    return 1;
            }
        }
    }
}
