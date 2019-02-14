using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class SmtpPasswordStepTest
    {
        private Job _job;

        [SetUp]
        public void Setup()
        {
            _job = new Job(null, new ConversionProfile(), new Accounts());
        }

        [Test]
        public void IsRequired_SmtpDisabled_ReturnsFalse()
        {
            var step = new SmtpPasswordStep();
            _job.Profile.EmailSmtpSettings.Enabled = false;

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_SmtpEnabledAndPasswordPresent_ReturnsFalse()
        {
            var step = new SmtpPasswordStep();
            _job.Profile.EmailSmtpSettings.Enabled = true;
            _job.Passwords.SmtpPassword = "SomeSmtpPassword";

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_SmtpEnabledAndMissingPassword_ReturnsTrue()
        {
            var step = new SmtpPasswordStep();
            _job.Profile.EmailSmtpSettings.Enabled = true;
            _job.Passwords.SmtpPassword = "";

            Assert.IsTrue(step.IsStepRequired(_job));
        }
    }
}
