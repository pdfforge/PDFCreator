using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class TimeServerAccountViewModel : AccountViewModelBase<TimeServerAccountInteraction, TimeServerTranslation>
    {
        private TimeServerAccount _timeServerAccount;

        public TimeServerAccountViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        { }

        public string Url
        {
            get { return _timeServerAccount?.Url; }
            set
            {
                _timeServerAccount.Url = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Username
        {
            get { return _timeServerAccount?.UserName; }
            set
            {
                _timeServerAccount.UserName = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _timeServerAccount?.Password; }
            set
            {
                _timeServerAccount.Password = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsSecured
        {
            get { return _timeServerAccount != null && _timeServerAccount.IsSecured; }
            set
            {
                _timeServerAccount.IsSecured = value;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(IsSecured));
            }
        }

        protected override void SaveExecute()
        {
            Interaction.TimeServerAccount = _timeServerAccount;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override bool SaveCanExecute()
        {
            // Url must be set
            if (string.IsNullOrWhiteSpace(Url))
                return false;

            if (_timeServerAccount.IsSecured)
            {
                // when authentication is set then we need a user name and a password
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                    return false;
            }

            return true;
        }

        protected override void HandleInteractionObjectChanged()
        {
            _timeServerAccount = Interaction.TimeServerAccount;

            RaisePropertyChanged(nameof(Url));
            RaisePropertyChanged(nameof(Username));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(IsSecured));
            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
