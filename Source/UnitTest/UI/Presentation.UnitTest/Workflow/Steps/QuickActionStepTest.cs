using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class QuickActionStepTest
    {
        private QuickActionStep _step;
        private Job _job;

        [SetUp]
        public void Setup()
        {
            _step = new QuickActionStep();
            _job = new Job(null, null, null);
            _job.Profile = new ConversionProfile();
        }

        [Test]
        public void CreateStep_Check_NavigationUriEqualsViewName()
        {
            Assert.AreEqual(nameof(QuickActionView), _step.NavigationUri);
        }

        [Test]
        public void CreateStep_CheckIfStepRequired_ReturnsSameValueAsShowQuickActionsFromTheJob()
        {
            _job.Profile.ShowQuickActions = true;
            Assert.IsTrue(_step.IsStepRequired(_job));
            _job.Profile.ShowQuickActions = false;
            Assert.IsFalse(_step.IsStepRequired(_job));
        }
    }
}
