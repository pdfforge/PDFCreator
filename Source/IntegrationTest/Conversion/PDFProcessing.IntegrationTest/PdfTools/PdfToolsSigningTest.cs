using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base;
using pdfforge.PDFCreator.Utilities;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.PdfTools
{
    public class PdfToolsSigningTest : SigningTestBase
    {
        private CertificateManager _certificateManager;
        private static readonly object LockObject = new object();

        protected override IPdfProcessor BuildPdfProcessor()
        {
            lock (LockObject)
            {
                if (_certificateManager == null)
                    _certificateManager = new CertificateManager();

                var pdfToolsLicensing = new PdfToolsTestLicensing();
                Assert.IsTrue(pdfToolsLicensing.Apply(), "Could not apply pdf-tools licensing.");

                return new PdfToolsPdfProcessor(new FileWrap(), _certificateManager, new VersionHelper(GetType().Assembly), new ApplicationNameProvider("ApplicationName"));
            }
        }

        protected override void FinalizePdfProcessor()
        {
            _certificateManager?.Dispose();
        }

        [Test]
        public void Signing_ExpiredLicense_ThrowsException()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFile_ExpiredP12);

            var ex = Assert.Throws<ProcessingException>(() => PdfProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_Invalid, ex.ErrorCode, "Wrong error code for expired certificate/signature");
        }
    }
}
