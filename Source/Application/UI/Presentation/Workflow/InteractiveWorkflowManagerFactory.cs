using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;
using pdfforge.PDFCreator.Utilities;
using Prism.Regions;
using System.Collections.Generic;
using pdfforge.PDFCreator.Utilities.Update;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IInteractiveWorkflowManagerFactory
    {
        InteractiveWorkflowManager CreateInteractiveWorkflowManager(IRegionManager regionManager, ICurrentSettingsProvider currentSettingsProvider);
    }

    public class InteractiveWorkflowManagerFactory : IInteractiveWorkflowManagerFactory
    {
        private readonly IWorkflowNavigationHelper _workflowNavigationHelper;
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
        private readonly IUpdateHelper _updateHelper;

        protected List<IWorkflowStep> WorkflowSteps;
        protected IErrorStep ErrorStep;

        public InteractiveWorkflowManagerFactory(IWorkflowNavigationHelper workflowNavigationHelper, ISignaturePasswordCheck signaturePasswordCheck, IUpdateHelper updateHelper)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _signaturePasswordCheck = signaturePasswordCheck;
            _updateHelper = updateHelper;
        }

        public virtual InteractiveWorkflowManager CreateInteractiveWorkflowManager(IRegionManager regionManager, ICurrentSettingsProvider currentSettingsProvider)
        {
            WorkflowSteps = new List<IWorkflowStep>();

            WorkflowSteps.Add(WorkflowStep.Create<PrintJobView>(job => !job.Profile.SkipPrintDialog));
            WorkflowSteps.Add(new PdfPasswordsStep());
            WorkflowSteps.Add(new FtpPasswordStep());
            WorkflowSteps.Add(new SmtpPasswordStep());
            WorkflowSteps.Add(new HttpPasswordStep());
            WorkflowSteps.Add(new SignaturePasswordStep(_signaturePasswordCheck));
            WorkflowSteps.Add(WorkflowStep.Create<ProgressView>());
            WorkflowSteps.Add(new UpdateHintStep(_updateHelper));
            WorkflowSteps.Add(new QuickActionStep());

            ErrorStep = new ErrorStep();

            return new InteractiveWorkflowManager(_workflowNavigationHelper, regionManager, WorkflowSteps, ErrorStep);
        }
    }

    public class InteractiveWorkflowManagerFactoryWithConditionalHintSteps : InteractiveWorkflowManagerFactory
    {
        private readonly IWorkflowNavigationHelper _workflowNavigationHelper;
        private readonly IConditionalHintManager _conditionalHintManager;

        public InteractiveWorkflowManagerFactoryWithConditionalHintSteps(IWorkflowNavigationHelper workflowNavigationHelper, IConditionalHintManager conditionalHintManager, ISignaturePasswordCheck signaturePasswordCheck, IUpdateHelper updateHelper)
            : base(workflowNavigationHelper, signaturePasswordCheck, updateHelper)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _conditionalHintManager = conditionalHintManager;
        }

        public override InteractiveWorkflowManager CreateInteractiveWorkflowManager(IRegionManager regionManager, ICurrentSettingsProvider currentSettingsProvider)
        {
            base.CreateInteractiveWorkflowManager(regionManager, currentSettingsProvider);
            WorkflowSteps.Add(new ProfessionalHintStep(_conditionalHintManager));
            WorkflowSteps.Add(new EmailCollectionHintStep(_conditionalHintManager));

            return new InteractiveWorkflowManager(_workflowNavigationHelper, regionManager, WorkflowSteps, ErrorStep);
        }
    }
}
