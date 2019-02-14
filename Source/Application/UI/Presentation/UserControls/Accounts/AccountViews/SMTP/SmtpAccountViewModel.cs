using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class SmtpAccountViewModel : AccountViewModelBase<SmtpAccountInteraction, SmtpTranslation>
    {
        private SmtpAccount _smtpAccount;

        public SmtpAccountViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        { }

        public string Server
        {
            get { return _smtpAccount?.Server; }
            set
            {
                _smtpAccount.Server = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected override void SaveExecute()
        {
            Interaction.SmtpAccount = _smtpAccount;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override bool SaveCanExecute()
        {
            return (AskForPasswordLater || !string.IsNullOrWhiteSpace(Password)) &&
                    !string.IsNullOrWhiteSpace(Address) &&
                   !string.IsNullOrWhiteSpace(Server) &&
                   !string.IsNullOrWhiteSpace(Username);
        }

        public string Address
        {
            get { return _smtpAccount?.Address; }
            set
            {
                _smtpAccount.Address = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Username
        {
            get { return _smtpAccount?.UserName; }
            set
            {
                _smtpAccount.UserName = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _smtpAccount?.Password; }
            set
            {
                _smtpAccount.Password = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public int Port
        {
            get { return _smtpAccount?.Port ?? 0; }
            set { _smtpAccount.Port = value; }
        }

        public bool Ssl
        {
            get { return _smtpAccount?.Ssl ?? false; }
            set { _smtpAccount.Ssl = value; }
        }

        protected override void HandleInteractionObjectChanged()
        {
            _smtpAccount = Interaction.SmtpAccount;

            RaisePropertyChanged(nameof(Address));
            RaisePropertyChanged(nameof(Server));
            RaisePropertyChanged(nameof(Username));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(Port));
            RaisePropertyChanged(nameof(Ssl));
            AskForPasswordLater = string.IsNullOrWhiteSpace(Password);
            RaisePropertyChanged(nameof(AskForPasswordLater));
            SaveCommand.RaiseCanExecuteChanged();
        }

        public void SetAdressForEmptyUsername()
        {
            if (string.IsNullOrEmpty(Username))
            {
                Username = Address;
                RaisePropertyChanged(nameof(Username));
            }
        }
    }
}
