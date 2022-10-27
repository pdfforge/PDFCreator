using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.Obsidian;

namespace pdfforge.PDFCreator.UI.Presentation.ViewModelBases
{
    public abstract class JobStepPasswordViewModelBase<T> : TranslatableViewModelBase<T>, IWorkflowViewModel
    where T : PasswordButtonControlTranslation, new()
    {
        protected Job Job;

        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                ContinueCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(Password));
            }
        }

        public DelegateCommand ContinueCommand { get; }
        public ICommand SkipCommand { get; }
        public ICommand CancelCommand { get; }

        protected JobStepPasswordViewModelBase(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            ContinueCommand = new DelegateCommand(ContinueExecute, ContinueCanExecute);
            SkipCommand = new DelegateCommand(SkipExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
        }

        public Task ExecuteWorkflowStep(Job job)
        {
            Job = job;
            InitializeWorkflowStep();

            return _taskCompletionSource.Task;
        }

        /// <summary>
        /// Set all properties of current workflow step
        /// </summary>
        protected abstract void InitializeWorkflowStep();

        protected virtual bool ContinueCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Password);
        }

        protected abstract void StorePasswordsInJobPasswords();

        private void ContinueExecute(object obj)
        {
            StorePasswordsInJobPasswords();
            Finish();
        }

        protected abstract void DisableAction();

        private void SkipExecute(object obj)
        {
            DisableAction();
            Finish();
        }

        private void CancelExecute(object obj)
        {
            Finish();

            var cancelMessage = "User cancelled in " + GetType().UnderlyingSystemType.Name;
            throw new AbortWorkflowException(cancelMessage);
        }

        private void Finish()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
            _taskCompletionSource.SetResult(null);
        }

        public event EventHandler StepFinished;
    }
}
