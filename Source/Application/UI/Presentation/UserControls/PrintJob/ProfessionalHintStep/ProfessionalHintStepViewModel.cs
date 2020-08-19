using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DelegateCommand = pdfforge.Obsidian.DelegateCommand;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.ProfessionalHintStep
{
    public class ProfessionalHintStepViewModel : TranslatableViewModelBase<ProfessionalHintStepTranslation>, IWorkflowViewModel
    {
        private readonly IProfessionalHintHelper _professionalHintHelper;
        private TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        public ProfessionalHintStepViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, IProfessionalHintHelper professionalHintHelper) : base(translationUpdater)
        {
            _professionalHintHelper = professionalHintHelper;
            FinishStepCommand = new DelegateCommand(o => CancelExecute());

            ProfessionalHintCommand = new CompositeCommand(); //compose "open plus hint url" and "call step finsihed"
            var urlOpenCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.ProfessionalHintUrl);
            if (urlOpenCommand != null) //required to avoid NullException in DesignTimeViewModel
            {
                ProfessionalHintCommand.RegisterCommand(urlOpenCommand);
            }

            ProfessionalHintCommand.RegisterCommand(new DelegateCommand(o => InvokeStepFinished()));
        }

        public CompositeCommand ProfessionalHintCommand { get; set; }

        public ICommand FinishStepCommand { get; set; }

        public event EventHandler StepFinished;

        public string ThankYouText => Translation.GetThankYouMessage(_professionalHintHelper.CurrentJobCounter);

        public Task ExecuteWorkflowStep(Job job)
        {
            return _taskCompletionSource.Task;
        }

        private void InvokeStepFinished()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
            _taskCompletionSource.SetResult(null);
        }

        private void CancelExecute()
        {
            InvokeStepFinished();
            throw new AbortWorkflowException("User cancelled in the PlusHintView");
        }
    }
}
