using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;
using pdfforge.PDFCreator.Utilities;
using Prism.Regions;
using System.Collections.Generic;

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
        private readonly IUpdateAssistant _updateAssistant;

        protected List<IWorkflowStep> WorkflowSteps;

        public InteractiveWorkflowManagerFactory(IWorkflowNavigationHelper workflowNavigationHelper, ISignaturePasswordCheck signaturePasswordCheck, IUpdateAssistant updateAssistant)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _signaturePasswordCheck = signaturePasswordCheck;
            _updateAssistant = updateAssistant;
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
            WorkflowSteps.Add(new DropboxSharedLinkStep());
            WorkflowSteps.Add(new UpdateHintStep(_updateAssistant));
            WorkflowSteps.Add(new QuickActionStep());

            return new InteractiveWorkflowManager(_workflowNavigationHelper, regionManager, WorkflowSteps);
        }
    }

    public class InteractiveWorkflowManagerFactoryWithPlusHintStep : InteractiveWorkflowManagerFactory
    {
        private readonly IWorkflowNavigationHelper _workflowNavigationHelper;
        private readonly IPlusHintHelper _plusHintHelper;

        public InteractiveWorkflowManagerFactoryWithPlusHintStep(IWorkflowNavigationHelper workflowNavigationHelper, IPlusHintHelper plusHintHelper, ISignaturePasswordCheck signaturePasswordCheck, IUpdateAssistant updateAssistant)
            : base(workflowNavigationHelper, signaturePasswordCheck, updateAssistant)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _plusHintHelper = plusHintHelper;
        }

        public override InteractiveWorkflowManager CreateInteractiveWorkflowManager(IRegionManager regionManager, ICurrentSettingsProvider currentSettingsProvider)
        {
            base.CreateInteractiveWorkflowManager(regionManager, currentSettingsProvider);
            WorkflowSteps.Add(new PlusHintStep(_plusHintHelper));

            return new InteractiveWorkflowManager(_workflowNavigationHelper, regionManager, WorkflowSteps);
        }
    }
}
