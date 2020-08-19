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
        private readonly IUpdateHelper _updateHelper;

        protected List<IWorkflowStep> WorkflowSteps;

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
            WorkflowSteps.Add(new DropboxSharedLinkStep());
            WorkflowSteps.Add(new UpdateHintStep(_updateHelper));
            WorkflowSteps.Add(new QuickActionStep());

            return new InteractiveWorkflowManager(_workflowNavigationHelper, regionManager, WorkflowSteps);
        }
    }

    public class InteractiveWorkflowManagerFactoryWithProfessionalHintHintStep : InteractiveWorkflowManagerFactory
    {
        private readonly IWorkflowNavigationHelper _workflowNavigationHelper;
        private readonly IProfessionalHintHelper _professionalHintHelper;

        public InteractiveWorkflowManagerFactoryWithProfessionalHintHintStep(IWorkflowNavigationHelper workflowNavigationHelper, IProfessionalHintHelper professionalHintHelper, ISignaturePasswordCheck signaturePasswordCheck, IUpdateHelper updateHelper)
            : base(workflowNavigationHelper, signaturePasswordCheck, updateHelper)
        {
            _workflowNavigationHelper = workflowNavigationHelper;
            _professionalHintHelper = professionalHintHelper;
        }

        public override InteractiveWorkflowManager CreateInteractiveWorkflowManager(IRegionManager regionManager, ICurrentSettingsProvider currentSettingsProvider)
        {
            base.CreateInteractiveWorkflowManager(regionManager, currentSettingsProvider);
            WorkflowSteps.Add(new ProfessionalHintStep(_professionalHintHelper));

            return new InteractiveWorkflowManager(_workflowNavigationHelper, regionManager, WorkflowSteps);
        }
    }
}
