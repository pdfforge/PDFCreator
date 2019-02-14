using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    internal abstract class PdfAValidationTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        [SetUp]
        public void SetUp()
        {
            _pdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            container.Options.AllowOverridingRegistrations = true;
            container.Register(() => _pdfProcessor);
            container.Options.AllowOverridingRegistrations = false;

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder($"PDFProcessing_{_pdfProcessor.GetType().Name}_PDFA");
        }

        private void InitializeTest(OutputFormat outputFormat)
        {
            _th.GenerateGsJob(PSfiles.PortraitPage, outputFormat);

            //Settings of the set outputfile
            _th.Job.JobInfo.Metadata.Title = "Test Title";
            _th.Job.JobInfo.Metadata.Subject = "Test Subject";
            _th.Job.JobInfo.Metadata.Keywords = "Test Keywords";
            _th.Job.JobInfo.Metadata.Author = "Test Author";
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
            FinalizePdfProcessor();
        }

        [Test]
        public void ValidatePdfA1B()
        {
            InitializeTest(OutputFormat.PdfA1B);
            _th.RunGsJob();
            PDFValidation.ValidatePdf(_th.Job);
        }

        [Test]
        public void ValidatePdfA2B()
        {
            InitializeTest(OutputFormat.PdfA2B);
            _th.RunGsJob();
            PDFValidation.ValidatePdf(_th.Job);
        }

        [Test]
        public void ValidatePdfA3B()
        {
            InitializeTest(OutputFormat.PdfA3B);
            _th.RunGsJob();
            PDFValidation.ValidatePdf(_th.Job);
        }

        [Test]
        public void ValidatePdfA3B_WithSignature()
        {
            InitializeTest(OutputFormat.PdfA3B);
            ApplySignatureSettings();
            _th.RunGsJob();
            PDFValidation.ValidatePdf(_th.Job);
        }

        private void ApplySignatureSettings()
        {
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;

            _th.Job.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);
            _th.Job.Passwords.PdfSignaturePassword = "Test1";

            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 1;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 50;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 50;
            _th.Job.Profile.PdfSettings.Signature.RightX = 300;
            _th.Job.Profile.PdfSettings.Signature.RightY = 150;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";

            var timeServerAccount = new TimeServerAccount();
            timeServerAccount.AccountId = "TestAccountId";
            timeServerAccount.Url = "http://timestamp.digicert.com";
            timeServerAccount.IsSecured = false;
            _th.Job.Accounts.TimeServerAccounts.Add(timeServerAccount);

            _th.Job.Profile.PdfSettings.Signature.TimeServerAccountId = timeServerAccount.AccountId;
        }
    }
}
