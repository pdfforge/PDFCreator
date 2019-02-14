using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Ploeh.AutoFixture;
using Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class InteractiveWorkflowManagerTest
    {
        private IRegionManager _regionManager;
        private IWorkflowNavigationHelper _workflowNavigationHelper;
        private Job _job;
        private Dictionary<IWorkflowStep, IWorkflowViewModel> _viewModelMapping;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());
            _regionManager = Substitute.For<IRegionManager>();
            _workflowNavigationHelper = Substitute.For<IWorkflowNavigationHelper>();
            _viewModelMapping = new Dictionary<IWorkflowStep, IWorkflowViewModel>();
            _fixture = new Fixture();
        }

        private InteractiveWorkflowManager BuildWorkflowManager(IEnumerable<IWorkflowStep> workflowSteps = null)
        {
            var steps = workflowSteps ?? new List<IWorkflowStep>();

            var workflowManager = new InteractiveWorkflowManager(_workflowNavigationHelper, _regionManager, steps);

            workflowManager.Job = _job;

            return workflowManager;
        }

        private IWorkflowStep BuildAndRegisterWorkflowStep(bool isRequired = true)
        {
            var step = Substitute.For<IWorkflowStep>();
            step.NavigationUri.Returns(_fixture.Create<string>());
            step.IsStepRequired(Arg.Any<Job>()).Returns(isRequired);

            var viewModel = Substitute.For<IWorkflowViewModel>();
            _workflowNavigationHelper.GetValidatedViewModel(Arg.Any<IRegion>(), step.NavigationUri).Returns(viewModel);

            _viewModelMapping[step] = viewModel;

            return step;
        }

        [Test]
        public async Task Run_WithDisabledStep_StepIsSkipped()
        {
            var step = BuildAndRegisterWorkflowStep(isRequired: false);
            var workflowManager = BuildWorkflowManager(new[] { step });

            await workflowManager.Run();

            await step.DidNotReceiveWithAnyArgs().ExecuteStep(null, null);
        }

        [Test]
        public async Task Run_WithSingleStep_ExecutesStep()
        {
            var step = BuildAndRegisterWorkflowStep();
            var viewModel = Substitute.For<IWorkflowViewModel>();
            _workflowNavigationHelper.GetValidatedViewModel(Arg.Any<IRegion>(), step.NavigationUri).Returns(viewModel);
            var workflowManager = BuildWorkflowManager(new[] { step });

            await workflowManager.Run();

            await step.Received().ExecuteStep(_job, viewModel);
        }

        [Test]
        public async Task Run_WithMultipleSteps_ExecutesSteps()
        {
            var steps = new[] { BuildAndRegisterWorkflowStep(), BuildAndRegisterWorkflowStep() };
            var workflowManager = BuildWorkflowManager(steps);

            await workflowManager.Run();

            foreach (var step in steps)
            {
                await step.Received().ExecuteStep(_job, _viewModelMapping[step]);
            }
        }
    }
}
