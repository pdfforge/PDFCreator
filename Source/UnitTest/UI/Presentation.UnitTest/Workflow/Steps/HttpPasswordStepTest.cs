using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class HttpPasswordStepTest
    {
        private Job _job;
        private HttpAccount _account;

        [SetUp]
        public void Setup()
        {
            _account = new HttpAccount();
            _account.AccountId = "TestID";

            var profile = new ConversionProfile();
            profile.HttpSettings.AccountId = _account.AccountId;

            var accounts = new Accounts();
            accounts.HttpAccounts.Add(_account);

            _job = new Job(null, profile, accounts);
        }

        [Test]
        public void IsRequired_HttpDisabled_ReturnsFalse()
        {
            var step = new HttpPasswordStep();
            _job.Profile.HttpSettings.Enabled = false;

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_HttpEnabledWithoutBasicAuthentication_ReturnsFalse()
        {
            var step = new HttpPasswordStep();
            _job.Profile.HttpSettings.Enabled = true;
            _account.IsBasicAuthentication = false;

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_HttpEnabledWithBasicAuthenticationAndPasswordPresent_ReturnsFalse()
        {
            var step = new HttpPasswordStep();
            _job.Profile.HttpSettings.Enabled = true;
            _account.IsBasicAuthentication = true;
            _job.Passwords.HttpPassword = "SomeHttpPassword";

            Assert.IsFalse(step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_HttpEnabledWithBasicAuthenticationAndMissingPassword_ReturnsTrue()
        {
            var step = new HttpPasswordStep();
            _job.Profile.HttpSettings.Enabled = true;
            _account.IsBasicAuthentication = true;
            _job.Passwords.HttpPassword = "";

            Assert.IsTrue(step.IsStepRequired(_job));
        }
    }
}
