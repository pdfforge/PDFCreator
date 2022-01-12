using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public class InteractiveWorkflowManager
    {
        private readonly IWorkflowNavigationHelper _workflowNavigationHelper;
        private readonly IRegionManager _regionManager;
        public Job Job { get; set; }
        public bool Cancel { private get; set; }

        private readonly List<IWorkflowStep> _steps = new List<IWorkflowStep>();
        private readonly IErrorStep _errorStep;

        public InteractiveWorkflowManager(IWorkflowNavigationHelper workflowNavigationHelper, IRegionManager regionManager, IEnumerable<IWorkflowStep> workflowSteps, IErrorStep errorStep)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _regionManager = regionManager;
            _errorStep = errorStep;

            _steps.AddRange(workflowSteps);
        }

        public async Task Run()
        {
            var region = _regionManager.Regions[PrintJobRegionNames.PrintJobMainRegion];

            foreach (var step in _steps)
            {
                if (Cancel)
                    return;

                if (!step.IsStepRequired(Job))
                    continue;

                region.RequestNavigate(step.NavigationUri);

                var viewModel = _workflowNavigationHelper.GetValidatedViewModel(region, step.NavigationUri);

                try
                {
                    await step.ExecuteStep(Job, viewModel);
                }
                catch (ProcessingException e)
                {
                    await HandleError(region, new ActionResult(e.ErrorCode), false);
                    throw;
                }
                catch (AggregateProcessingException e)
                {
                    await HandleError(region, e.Result, true);
                }
            }
        }

        private async Task HandleError(IRegion region, ActionResult result, bool formatAsWarning)
        {
            region.RequestNavigate(_errorStep.NavigationUri);
            var viewModel = _workflowNavigationHelper.GetValidatedErrorViewModel(region, _errorStep.NavigationUri);
            await _errorStep.ExecuteStep(Job, viewModel, result, formatAsWarning);
        }
    }
}
