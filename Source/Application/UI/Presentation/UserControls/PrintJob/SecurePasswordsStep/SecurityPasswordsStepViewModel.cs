using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SecurityPasswordsStepViewModel : JobStepPasswordViewModelBase<SecurityPasswordsStepTranslation>
    {
        private string _ownerPassword = "";
        private string _userPassword = "";
        private bool _askOwnerPassword = true;
        private bool _askUserPassword = true;

        public bool AskOwnerPassword
        {
            get { return _askOwnerPassword; }
            set { SetProperty(ref _askOwnerPassword, value); }
        }

        public bool AskUserPassword
        {
            get { return _askUserPassword; }
            set { SetProperty(ref _askUserPassword, value); }
        }

        public string OwnerPassword
        {
            get { return _ownerPassword; }
            set
            {
                SetProperty(ref _ownerPassword, value);
                ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                SetProperty(ref _userPassword, value);
                ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        public SecurityPasswordsStepViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void DisableAction()
        {
            Job.Profile.PdfSettings.Security.Enabled = false;
        }

        protected override void InitializeWorkflowStep()
        {
            var securitySettings = Job.Profile.PdfSettings.Security;
            AskOwnerPassword = securitySettings.OwnerPassword == "";
            AskUserPassword = securitySettings.RequireUserPassword && securitySettings.UserPassword == "";
        }

        protected override void StorePasswordsInJobPasswords()
        {
            Job.Passwords.PdfUserPassword = UserPassword;
            Job.Passwords.PdfOwnerPassword = OwnerPassword;
        }

        protected override bool ContinueCanExecute(object obj)
        {
            if (AskOwnerPassword)
                if (string.IsNullOrWhiteSpace(OwnerPassword))
                    return false;

            if (AskUserPassword)
                if (string.IsNullOrWhiteSpace(UserPassword))
                    return false;

            return true;
        }
    }
}
