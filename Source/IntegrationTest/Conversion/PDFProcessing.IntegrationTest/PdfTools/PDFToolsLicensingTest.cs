using NUnit.Framework;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.PdfTools
{
    [TestFixture]
    public class PDFToolsLicensingTest
    {
        [Test]
        public void Apply_AllKeysAreValid_ReturnsTrue()
        {
            var pdfToolsLicensing = new PdfToolsTestLicensing();

            Assert.IsTrue(pdfToolsLicensing.Apply(), "Could not apply pdf-tools licensing.");
        }

        [Test]
        public void Apply_InvalidToolboxKey_ReturnsFalse()
        {
            var pdfToolsLicensing = new PdfToolsTestLicensing();
            pdfToolsLicensing._PdfToolboxKey = "Invalid ToolboxKey";
            Assert.IsFalse(pdfToolsLicensing.Apply());
        }

        [Test]
        public void Apply_InvalidPdfaConverterKey_ReturnsFalse()
        {
            var pdfToolsLicensing = new PdfToolsTestLicensing();
            pdfToolsLicensing._PdfAConverterKey = "Invalid PdfaConverterKey";
            Assert.IsFalse(pdfToolsLicensing.Apply());
        }

        [Test]
        public void Apply_InvalidPdfSecureKey_ReturnsFalse()
        {
            var pdfToolsLicensing = new PdfToolsTestLicensing();
            pdfToolsLicensing._PdfSecureKey = "Invalid PdfSecureKey";
            Assert.IsFalse(pdfToolsLicensing.Apply());
        }

        [Test]
        public void Apply_InvalidPdfAValidatorKey_ReturnsFalse()
        {
            var pdfToolsLicensing = new PdfToolsTestLicensing();
            pdfToolsLicensing._PdfAValidatorKey = "Invalid PdfAValidatorKey";
            Assert.IsFalse(pdfToolsLicensing.Apply());
        }
    }
}
