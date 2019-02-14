using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using SimpleInjector;

namespace pdfforge.PDFCreator.PerformanceTest.Conversion.PDFProcessing
{
    #region Combinatorial Enums

    public enum Encryption
    {
        EncryptionEnabled,
        EncryptionDisabled
    }

    public enum Signing
    {
        SigningEnabled,
        SigningDisabled
    }

    public enum Background
    {
        BackgroundEnabled,
        BackgroundDisabled
    }

    public enum BasicPermissions
    {
        AllowBasicPermissions,
        DenyBasicPermissions,
    }

    public enum ExtendedPermissions
    {
        AllowExtededPermissions,
        DenyExtededPermissions,
    }

    #endregion Combinatorial Enums

    internal class ITextCombinatorialTest : PdfProcessingCombinatorialTest
    {
        protected override bool IsIText => true;

        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, ITextPdfProcessor>();
        }

        protected override void FinalizePdfProcessor()
        { }
    }

    [TestFixture]
    internal abstract class PdfProcessingCombinatorialTest
    {
        protected abstract bool IsIText { get; }

        protected abstract void RegisterPdfProcessor(Container container);

        protected abstract void FinalizePdfProcessor();

        private IPdfProcessor _pdfProcessor;

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();

            //override PasswordProvider, because tests write to job.passwords directly and the provider would replace those values with empty strings from the profile
            container.Options.AllowOverridingRegistrations = true;
            RegisterPdfProcessor(container);

            _pdfProcessor = container.GetInstance<IPdfProcessor>();

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PdfProcessing PerformanceTest");

            var timeServerAccount = new TimeServerAccount();
            timeServerAccount.AccountId = "TestTimerserverID";
            var accounts = new Accounts();
            accounts.TimeServerAccounts.Add(timeServerAccount);
            _th.SetAccounts(accounts);

            _th.Profile.PdfSettings.Signature.TimeServerAccountId = timeServerAccount.AccountId;

            _th.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);
            _th.Profile.BackgroundPage.File = _th.GenerateTestFile(TestFile.Background3PagesPDF);
        }

        [TearDown]
        public void CleanUp()
        {
            if (_th != null)
                _th.CleanUp();
            FinalizePdfProcessor();
        }

        private const string TestCertPw = "Test1";
        private TestHelper _th;

        private void MakePasswordTests(OutputFormat format)
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, format);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.RunGsJob();
            EncryptionTester.DoPasswordTest(_th.Job);

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, format);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.Job.Passwords.PdfUserPassword = "TestUserPw";
            _th.Job.Passwords.PdfOwnerPassword = "TestOwnerPw";
            _th.RunGsJob();
            EncryptionTester.DoPasswordTest(_th.Job);

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, format);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.Job.Passwords.PdfUserPassword = "TestUserPw";
            _th.Job.Passwords.PdfOwnerPassword = "TestOwnerPw";
            _th.RunGsJob();
            EncryptionTester.DoPasswordTest(_th.Job);
        }

        private void DoAllTests()
        {
            var format = _th.Job.Profile.OutputFormat;

            PdfVersionTester.CheckPDFVersion(_th.Job, _pdfProcessor);

            SigningTester.TestSignature(_th.Job);

            if (_th.Profile.BackgroundPage.Enabled)
                BackgroundPageTester.BackgroundOnPage(_th.Job);

            /*
            if (format == OutputFormat.PdfA1B || format == OutputFormat.PdfA2B)
                XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            */

            if (format != OutputFormat.PdfX)
                PDFValidation.ValidatePdf(_th.Job);

            //Must be last because it changes the encryption
            if ((format == OutputFormat.Pdf) && (_th.Profile.PdfSettings.Security.Enabled))
            {
                EncryptionTester.DoSecurityTest(_th.Job, IsIText);
                MakePasswordTests(_th.Job.Profile.OutputFormat);
            }
        }

        [Test, Combinatorial]
        public void CombinatorialTest_PDF_Practicable(
            [Values(Signing.SigningEnabled, Signing.SigningDisabled)] Signing signing,
            [Values(Background.BackgroundEnabled, Background.BackgroundDisabled)] Background background,
            [Values(Encryption.EncryptionEnabled, Encryption.EncryptionDisabled)] Encryption encryption,
            [Values(EncryptionLevel.Rc40Bit, EncryptionLevel.Aes128Bit, EncryptionLevel.Aes128Bit, EncryptionLevel.Aes256Bit)] EncryptionLevel encryptionLevel,
            [Values(BasicPermissions.AllowBasicPermissions, BasicPermissions.DenyBasicPermissions)] BasicPermissions basicPermissions,
            [Values(ExtendedPermissions.AllowExtededPermissions, ExtendedPermissions.DenyExtededPermissions)] ExtendedPermissions extendedPermissions
        )
        {
            _th.Profile.PdfSettings.Signature.Enabled = signing == Signing.SigningEnabled;

            _th.Profile.BackgroundPage.Enabled = background == Background.BackgroundEnabled;

            _th.Profile.PdfSettings.Security.Enabled = encryption == Encryption.EncryptionEnabled;
            _th.Profile.PdfSettings.Security.EncryptionLevel = encryptionLevel;

            _th.Profile.PdfSettings.Security.AllowToCopyContent = basicPermissions == BasicPermissions.AllowBasicPermissions;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = basicPermissions == BasicPermissions.AllowBasicPermissions;
            _th.Profile.PdfSettings.Security.AllowPrinting = basicPermissions == BasicPermissions.AllowBasicPermissions;
            _th.Profile.PdfSettings.Security.AllowToEditComments = basicPermissions == BasicPermissions.AllowBasicPermissions;

            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = extendedPermissions == ExtendedPermissions.AllowExtededPermissions;
            _th.Profile.PdfSettings.Security.AllowToFillForms = extendedPermissions == ExtendedPermissions.AllowExtededPermissions;
            _th.Profile.PdfSettings.Security.AllowScreenReader = extendedPermissions == ExtendedPermissions.AllowExtededPermissions;
            _th.Profile.PdfSettings.Security.AllowToEditAssembly = extendedPermissions == ExtendedPermissions.AllowExtededPermissions;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();

            DoAllTests();
        }

        [Test, Combinatorial, Description("Note: Some Tests are currently failing for IText")]
        public void CombinatorialTest(
            [Values(OutputFormat.PdfA1B, OutputFormat.PdfA2B, OutputFormat.PdfX)] OutputFormat format,
            [Values(Signing.SigningEnabled, Signing.SigningDisabled)] Signing signing,
            [Values(Background.BackgroundEnabled, Background.BackgroundDisabled)] Background background
        )
        {
            _th.Profile.PdfSettings.Signature.Enabled = signing == Signing.SigningEnabled;

            _th.Profile.BackgroundPage.Enabled = background == Background.BackgroundEnabled;

            _th.Profile.PdfSettings.Security.Enabled = false;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, format);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();

            DoAllTests();
        }
    }
}
