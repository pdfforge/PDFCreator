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

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint
{
    public class PlusHintViewModel : TranslatableViewModelBase<PlusHintViewTranslation>, IWorkflowViewModel
    {
        private readonly IPlusHintHelper _plusHintHelper;
        private TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        public PlusHintViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, IPlusHintHelper plusHintHelper) : base(translationUpdater)
        {
            _plusHintHelper = plusHintHelper;
            FinishStepCommand = new DelegateCommand(o => CancelExecute());

            PlusButtonCommand = new CompositeCommand(); //compose "open plus hint url" and "call step finsihed"
            var urlOpenCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PlusHintLink);
            if (urlOpenCommand != null) //required to avoid NullException in DesignTimeViewModel
            {
                PlusButtonCommand.RegisterCommand(urlOpenCommand);
            }

            PlusButtonCommand.RegisterCommand(new DelegateCommand(o => InvokeStepFinished()));
        }

        public CompositeCommand PlusButtonCommand { get; set; }

        public ICommand FinishStepCommand { get; set; }

        public event EventHandler StepFinished;

        public string ThankYouText => Translation.GetThankYouMessage(_plusHintHelper.CurrentJobCounter);

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
