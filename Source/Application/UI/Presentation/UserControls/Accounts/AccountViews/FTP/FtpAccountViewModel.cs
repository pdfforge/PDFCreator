using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class FtpAccountViewModel : AccountViewModelBase<FtpAccountInteraction, AccountsTranslation>
    {
        private FtpAccount _ftpAccount;

        public FtpAccountViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        public string Password
        {
            get { return _ftpAccount?.Password; }
            set { _ftpAccount.Password = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string Server
        {
            get { return _ftpAccount?.Server; }
            set
            {
                _ftpAccount.Server = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserName
        {
            get { return _ftpAccount?.UserName; }
            set
            {
                _ftpAccount.UserName = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected override bool SaveCanExecute()
        {
            return (AskForPasswordLater || !string.IsNullOrWhiteSpace(Password))
                   && !string.IsNullOrWhiteSpace(Server)
                   && !string.IsNullOrWhiteSpace(UserName);
        }

        protected override void SaveExecute()
        {
            Interaction.FtpAccount = _ftpAccount;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            _ftpAccount = Interaction.FtpAccount;

            RaisePropertyChanged(nameof(Server));
            RaisePropertyChanged(nameof(UserName));
            RaisePropertyChanged(nameof(Password));
            AskForPasswordLater = string.IsNullOrWhiteSpace(Password);
            RaisePropertyChanged(nameof(AskForPasswordLater));
            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
