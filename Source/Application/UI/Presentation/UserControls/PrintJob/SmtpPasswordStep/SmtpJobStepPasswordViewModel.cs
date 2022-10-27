using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SmtpJobStepPasswordViewModel : JobStepPasswordViewModelBase<SmtpPasswordStepTranslation>
    {
        public string SmtpAccountInfo { get; set; }

        public SmtpJobStepPasswordViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void DisableAction()
        {
            Job.Profile.EmailSmtpSettings.Enabled = false;
        }

        protected override void InitializeWorkflowStep()
        {
            SmtpAccountInfo = Job.Accounts.GetSmtpAccount(Job.Profile)?.AccountInfo;
            RaisePropertyChanged(nameof(SmtpAccountInfo));
            Password = Job.Passwords.SmtpPassword;
        }

        protected override bool ContinueCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Password);
        }

        protected override void StorePasswordsInJobPasswords()
        {
            Job.Passwords.SmtpPassword = Password;
        }
    }
}
