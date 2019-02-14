using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class WorkflowStepTest
    {
        private IWorkflowViewModel _workflowViewModel;
        private Action<Job> _handleJobAction = job => { };

        [SetUp]
        public void Setup()
        {
            _workflowViewModel = Substitute.For<IWorkflowViewModel>();
            _workflowViewModel.ExecuteWorkflowStep(Arg.Any<Job>()).Returns(x =>
            {
                _handleJobAction(x.Arg<Job>());
                return Task.FromResult((object)null);
            });
        }

        [Test]
        public void Create_WithoutPredicate_StepIsRequired()
        {
            var step = WorkflowStep.Create<UserControl>();

            Assert.IsTrue(step.IsStepRequired(null));
        }

        [Test]
        public void Create_NavigationUri_IsTypeName()
        {
            var step = WorkflowStep.Create<UserControl>();

            Assert.AreEqual(nameof(UserControl), step.NavigationUri);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithPredicate_EvaluatesPredicate(bool isRequired)
        {
            var step = WorkflowStep.Create<UserControl>(job => isRequired);

            Assert.AreEqual(isRequired, step.IsStepRequired(null));
        }

        [Test]
        public async Task ExecuteStep_ExecutesViewModelStep()
        {
            var stepWasExecuted = false;
            _handleJobAction = job => stepWasExecuted = true;
            var step = WorkflowStep.Create<UserControl>();

            await step.ExecuteStep(null, _workflowViewModel);

            Assert.IsTrue(stepWasExecuted);
        }
    }
}
