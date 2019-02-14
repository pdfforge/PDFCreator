using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base;
using pdfforge.PDFCreator.Utilities;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.PdfTools
{
    internal class PdfToolsBasicTest : PdfProcessingBasicTestBase
    {
        private ICertificateManager _certificateManager;

        protected override IPdfProcessor BuildPdfProcessor()
        {
            _certificateManager = new CertificateManager();

            var pdfToolsLicensing = new PdfToolsTestLicensing();
            Assert.IsTrue(pdfToolsLicensing.Apply(), "Could not apply pdf-tools licensing.");

            return new PdfToolsPdfProcessor(new FileWrap(), _certificateManager, new VersionHelper(GetType().Assembly), new ApplicationNameProvider("ApplicationName"));
        }

        protected override void FinalizePdfProcessor()
        {
            _certificateManager?.Dispose();
        }
    }
}
