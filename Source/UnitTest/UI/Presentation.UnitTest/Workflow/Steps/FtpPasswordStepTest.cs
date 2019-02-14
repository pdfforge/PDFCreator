using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class FtpPasswordStepTest
    {
        private Job _job;

        [SetUp]
        public void Setup()
        {
            _job = new Job(null, new ConversionProfile(), new Accounts());
        }

        [Test]
        public void IsRequired_FtpDisabled_ReturnsFalse()
        {
            var step = new FtpPasswordStep();
            _job.Profile.Ftp.Enabled = false;

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_FtpEnabledAndPasswordPresent_ReturnsFalse()
        {
            var step = new FtpPasswordStep();
            _job.Profile.Ftp.Enabled = true;
            _job.Passwords.FtpPassword = "SomeFtpPassword";

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_FtpEnabledAndMissingPassword_ReturnsTrue()
        {
            var step = new FtpPasswordStep();
            _job.Profile.Ftp.Enabled = true;
            _job.Passwords.FtpPassword = "";

            Assert.IsTrue(step.IsStepRequired(_job));
        }
    }
}
