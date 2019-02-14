using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    [Category("Brittle")]
    public abstract class SigningTestBase
    {
        protected TestHelper TestHelper;
        protected IPdfProcessor PdfProcessor;
        private TimeServerAccount _timeServerAccount;
        private Accounts _accounts;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        [SetUp]
        public void SetUp()
        {
            PdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _accounts = new Accounts();
            _timeServerAccount = new TimeServerAccount();
            _accounts.TimeServerAccounts.Add(_timeServerAccount);

            TestHelper = container.GetInstance<TestHelper>();
            TestHelper.InitTempFolder($"PDFProcessing_{PdfProcessor.GetType().Name}_Signing");

            TestHelper.GenerateGsJob_WithSetOutput(TestFile.ThreePDFCreatorTestpagesPDF);
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            ApplySignatureSettings();
        }

        private void ApplySignatureSettings()
        {
            TestHelper.Job.Accounts = _accounts;
            TestHelper.Job.Profile.PdfSettings.Signature.Enabled = true;

            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFileP12);
            TestHelper.Job.Passwords.PdfSignaturePassword = "Test1";

            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 1;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftX = 50;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftY = 50;
            TestHelper.Job.Profile.PdfSettings.Signature.RightX = 300;
            TestHelper.Job.Profile.PdfSettings.Signature.RightY = 150;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";

            _timeServerAccount.AccountId = "TestAccountId";
            _timeServerAccount.Url = "http://timestamp.digicert.com";
            _timeServerAccount.IsSecured = false;

            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerAccountId = _timeServerAccount.AccountId;
        }

        [TearDown]
        public void CleanUp()
        {
            TestHelper.CleanUp();
            FinalizePdfProcessor();
        }

        #region TimeServer

        private void TestTimeServer(string timeserverUrl)
        {
            // This test can be brittle, so we retry it first to make this more resilient to network issues
            Retry.Do(() => DoTestTimeServer(timeserverUrl), TimeSpan.FromSeconds(5), retryCount: 5);
        }

        private void DoTestTimeServer(string timeserverUrl)
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _timeServerAccount.Url = timeserverUrl;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void TimeServer_GlobalSign()
        {
            TestTimeServer("http://timestamp.globalsign.com/scripts/timestamp.dll");
        }

        [Test]
        public void TimeServer_DigiCert()
        {
            TestTimeServer("http://timestamp.digicert.com");
        }

        [Test, Ignore("FreeTSA is currently down")]
        public void TimeServer_FreeTSA()
        {
            TestTimeServer("https://freetsa.org/tsr");
        }

        #endregion TimeServer

        #region PDF

        [Test]
        public void SigningPdf_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _timeServerAccount.Url = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "UserName";
            _timeServerAccount.Password = "TimeServerPassword";

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void SigningPdf_CustomPageGreaterThanNumberOfPages()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void SigningPdf_CustomPageSpecialCharacters()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void SigningPdf_FirstPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void SigningPdf_Invisible()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void SigningPdf_LastPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        [Test]
        public void SigningPdf_SigningWasSuccessful_TokensWereReplacedInSignatureMetadata()
        {
            var tokenKey = TestHelper.Job.TokenReplacer.GetTokenNames(true)[0];
            var tokenValue = TestHelper.Job.TokenReplacer.ReplaceTokens(tokenKey);

            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = tokenKey;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = tokenKey;
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = tokenKey;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            // set expected values with replaced tokens before testing
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = tokenValue;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = tokenValue;
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = tokenValue;

            SigningTester.TestSignature(TestHelper.Job);
        }

        [Test]
        public void SigningPdf_UnavailableTimeServer_ThrowsProcessingException()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            _timeServerAccount.Url = "http://timestamp.HOPEFULLY.NEVER.EXISTS/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PdfProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        public void SigningPdf_WrongSignaturePassword_ThrowsProcessingException()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Passwords.PdfSignaturePassword = "Invalid certificate Password";

            var ex = Assert.Throws<ProcessingException>(() => PdfProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_WrongCertificatePassword, ex.ErrorCode, "Wrong error code for wrong certificate password");
        }

        [Test, Explicit("This has to be checked manually")]
        public void SigningPdf_TwoSignatures_MultisigningIsDisabled_FirstSignatureIsInvalid()
        {
            if (!Debugger.IsAttached)
                return;

            // TODO make multi-signing testable

            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PdfProcessor);

            Process.Start(multiSignedFile);
            Debugger.Break();
        }

        [Test, Explicit("This has to be checked manually")]
        public void SigningPdf_TwoSignatures_MultisigningIsEnabled_FirstSignatureIsValid()
        {
            if (!Debugger.IsAttached)
                return;

            // TODO make multi-signing testable

            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PdfProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PdfProcessor);

            Process.Start(multiSignedFile);
            Debugger.Break();
        }

        #endregion PDF

        #region PDF/X

        [Test]
        public void SigningPdfX()
        {
            TestHelper.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF_X);
            ApplySignatureSettings();

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        #endregion PDF/X

        #region PDF/A1-b

        [Test]
        public void SigningPdfA1B()
        {
            TestHelper.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF_A_1b);
            ApplySignatureSettings();

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        #endregion PDF/A1-b

        #region PDF/A2-b

        [Test]
        public void SigningPdfA2B()
        {
            TestHelper.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF_A_2b);
            ApplySignatureSettings();

            PdfProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PdfProcessor);
        }

        #endregion PDF/A2-b

        #region Parallel

        [Test]
        public void Test_Parallel_Signing()
        {
            // This test can be brittle, so we retry it first to make this more resilient to issues
            Retry.Do(DoParallelSigning, TimeSpan.FromSeconds(5), retryCount: 5);
        }

        private void DoParallelSigning()
        {
            _timeServerAccount.Url = "http://timestamp.digicert.com";

            var tmpfolder = Path.Combine(TestHelper.TmpTestFolder, "output");
            int numTests = 10;

            Directory.CreateDirectory(tmpfolder);
            foreach (var file in Directory.EnumerateFiles(tmpfolder, "*.pdf"))
            {
                File.Delete(file);
            }

            var jobs = new List<Job>();

            for (int i = 0; i < numTests; i++)
            {
                var pdfFile = Path.Combine(tmpfolder, $"{i:D5}.pdf");
                File.Copy(TestHelper.Job.TempOutputFiles[0], pdfFile);

                var job = new Job(new JobInfo(), TestHelper.Job.Profile, _accounts);
                job.Passwords = TestHelper.Job.Passwords;
                job.TempOutputFiles.Add(pdfFile);

                jobs.Add(job);
            }

            Parallel.ForEach(jobs, job =>
            {
                var processor = BuildPdfProcessor();
                processor.ProcessPdf(job);
                job.OutputFiles.Add(job.TempOutputFiles.First());
            });

            jobs.ForEach(SigningTester.TestSignature);

            FinalizePdfProcessor();
        }

        #endregion Parallel
    }
}
