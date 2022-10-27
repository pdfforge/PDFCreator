using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class HttpPasswordStepViewModel : JobStepPasswordViewModelBase<HttpPasswordStepTranslation>
    {
        public string HttpAccountInfo { get; set; }

        public HttpPasswordStepViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void DisableAction()
        {
            Job.Profile.HttpSettings.Enabled = false;
        }

        protected override void InitializeWorkflowStep()
        {
            HttpAccountInfo = Job.Accounts.GetHttpAccount(Job.Profile).AccountInfo;
            RaisePropertyChanged(nameof(HttpAccountInfo));

            Password = Job.Passwords.HttpPassword;
        }

        protected override bool ContinueCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Password);
        }

        protected override void StorePasswordsInJobPasswords()
        {
            Job.Passwords.HttpPassword = Password;
        }
    }
}
