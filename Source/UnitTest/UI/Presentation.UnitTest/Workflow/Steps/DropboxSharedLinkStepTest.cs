using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class DropboxSharedLinkStepTest
    {
        private Job _job;
        private DropboxSharedLinkStep _step;

        [SetUp]
        public void Setup()
        {
            _job = new Job(null, new ConversionProfile(), new Accounts());
            _step = new DropboxSharedLinkStep();

            _job.Profile.DropboxSettings.Enabled = true;
            _job.Profile.DropboxSettings.CreateShareLink = true;
            _job.ShareLinks.DropboxShareUrl = "Not empty";
        }

        [Test]
        public void NavigationUri_IsCorrect()
        {
            Assert.AreEqual(nameof(DropboxShareLinkStepView), _step.NavigationUri);
        }

        [Test]
        public void IsRequired_DropboxEnabledWithShareLink_DropboxShreUrlInJobIsNotEmpty_ReturnsTrue()
        {
            Assert.IsTrue(_step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_DropboxEnabledWithShareLink_DropboxShreUrlInJobIsEmpty_ReturnsFalse()
        {
            _job.ShareLinks.DropboxShareUrl = "";
            Assert.IsFalse(_step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_DropboxDisabled_ReturnsFalse()
        {
            _job.Profile.DropboxSettings.Enabled = false;

            Assert.IsFalse(_step.IsStepRequired(_job));
        }

        [Test]
        public void IsRequired_DropboxShareLinkDisabled_ReturnsFalse()
        {
            _job.Profile.DropboxSettings.CreateShareLink = false;

            Assert.IsFalse(_step.IsStepRequired(_job));
        }
    }
}
