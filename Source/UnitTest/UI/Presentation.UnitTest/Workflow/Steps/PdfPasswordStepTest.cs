using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class PdfPasswordStepTest
    {
        private Job _job;

        [SetUp]
        public void Setup()
        {
            _job = new Job(null, new ConversionProfile(), new Accounts());
        }

        [Test]
        public void IsStepRequired_WithDefaults_IsFalse()
        {
            var step = new PdfPasswordsStep();

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsStepRequired_WithoutPassowrds_IsTrue()
        {
            _job.Profile.PdfSettings.Security = new Security()
            {
                Enabled = true
            };

            var step = new PdfPasswordsStep();

            Assert.IsTrue(step.IsStepRequired(_job));
        }

        [Test]
        public void IsStepRequired_WithOwnerPassowrds_IsTrue()
        {
            _job.Profile.PdfSettings.Security = new Security()
            {
                Enabled = true,
                OwnerPassword = "Some Password"
            };

            var step = new PdfPasswordsStep();

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsStepRequired_WithUserpPasswordandEmptyPassowrds_IsTrue()
        {
            _job.Profile.PdfSettings.Security = new Security()
            {
                Enabled = true,
                RequireUserPassword = true
            };

            var step = new PdfPasswordsStep();

            Assert.IsTrue(step.IsStepRequired(_job));
        }

        [Test]
        public void IsStepRequired_WithUserpPasswordandEmptyPassowrds_IsFalse()
        {
            _job.Profile.PdfSettings.Security = new Security()
            {
                Enabled = true,
                RequireUserPassword = true,
                OwnerPassword = "Some Password",
                UserPassword = "My user password"
            };

            var step = new PdfPasswordsStep();

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsStepRequired_WithFormatNotPdf_IsFalse()
        {
            _job.Profile.PdfSettings.Security = new Security()
            {
                Enabled = true
            };
            _job.Profile.OutputFormat = OutputFormat.Png;

            var step = new PdfPasswordsStep();

            Assert.IsFalse(step.IsStepRequired(_job));
        }
    }
}
