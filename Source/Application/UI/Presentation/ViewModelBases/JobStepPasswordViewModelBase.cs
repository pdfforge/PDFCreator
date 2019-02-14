using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.ViewModelBases
{
    public abstract class JobStepPasswordViewModelBase<T> : TranslatableViewModelBase<T>, IPasswordButtonViewModel, IWorkflowViewModel
        where T : ITranslatable, new()
    {
        protected Job Job;

        private TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        private readonly string SettingPropertyName;
        public PasswordButtonController PasswordButtonController { get; }

        public string Password
        {
            get { return PasswordButtonController.Password; }
            set { PasswordButtonController.Password = value; }
        }

        protected JobStepPasswordViewModelBase(ITranslationUpdater translationUpdater, string settingPropertyName) : base(translationUpdater)
        {
            SettingPropertyName = settingPropertyName;
            PasswordButtonController = new PasswordButtonController(translationUpdater, this, true, false);
        }

        public Task ExecuteWorkflowStep(Job job)
        {
            Job = job;
            ReadPassword();
            ExecuteWorkflow();

            return _taskCompletionSource.Task;
        }

        protected abstract void ExecuteWorkflow();

        protected virtual void ReadPassword()
        {
            Password = string.Empty;
        }

        public virtual void SetPassword(string values)
        {
            var propertyInfo = Job.Passwords.GetType().GetProperty(SettingPropertyName);
            if (propertyInfo != null)
                propertyInfo.SetValue(Job.Passwords, values);
        }

        public virtual void SkipHook()
        {
        }

        public void CancelHook()
        {
            throw new AbortWorkflowException(GetCancelErrorMessage());
        }

        protected virtual string GetCancelErrorMessage()
        {
            return string.Empty;
        }

        public virtual void OkHook()
        {
        }

        public virtual void RemoveHook()
        {
            throw new NotImplementedException();
        }

        public virtual bool CanExecuteHook()
        {
            if (string.IsNullOrWhiteSpace(Password))
                return false;
            return true;
        }

        public event EventHandler StepFinished;

        public virtual void ClearPasswordFields()
        {
            Password = String.Empty;
        }

        public virtual void FinishedHook()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
            _taskCompletionSource.SetResult(null);
        }
    }
}
