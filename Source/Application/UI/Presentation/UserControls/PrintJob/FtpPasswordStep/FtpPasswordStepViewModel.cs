using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class FtpPasswordStepViewModel : JobStepPasswordViewModelBase<FtpPasswordStepTranslation>
    {
        public string FtpAccountInfo { get; set; }

        public FtpPasswordStepViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void DisableAction()
        {
            Job.Profile.Ftp.Enabled = false;
        }

        protected override void InitializeWorkflowStep()
        {
            FtpAccountInfo = Job.Accounts.GetFtpAccount(Job.Profile).AccountInfo;
            RaisePropertyChanged(nameof(FtpAccountInfo));

            Password = Job.Passwords.FtpPassword;
        }

        protected override void StorePasswordsInJobPasswords()
        {
            Job.Passwords.FtpPassword = Password;
        }

        protected override bool ContinueCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Password);
        }
    }
}
