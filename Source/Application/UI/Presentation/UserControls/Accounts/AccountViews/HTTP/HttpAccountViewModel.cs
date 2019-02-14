using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class HttpAccountViewModel : AccountViewModelBase<HttpAccountInteraction, HttpTranslation>
    {
        private HttpAccount _httpAccount;

        public HttpAccountViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        { }

        public string Url
        {
            get { return _httpAccount?.Url; }
            set
            {
                _httpAccount.Url = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Username
        {
            get { return _httpAccount?.UserName; }
            set
            {
                _httpAccount.UserName = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public int Timeout
        {
            get
            {
                if (_httpAccount != null)
                    return _httpAccount.Timeout;

                return 60;
            }
            set
            {
                _httpAccount.Timeout = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _httpAccount?.Password; }
            set
            {
                _httpAccount.Password = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasBasicAuthentication
        {
            get { return _httpAccount != null && _httpAccount.IsBasicAuthentication; }
            set
            {
                _httpAccount.IsBasicAuthentication = value;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(HasBasicAuthentication));
            }
        }

        protected override void SaveExecute()
        {
            Interaction.HttpAccount = _httpAccount;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override bool SaveCanExecute()
        {
            // Url must be set
            if (string.IsNullOrWhiteSpace(Url))
                return false;

            Uri isValidUrl;
            if (!Uri.TryCreate(Url, UriKind.Absolute, out isValidUrl))
                return false;
            if (isValidUrl.Scheme != Uri.UriSchemeHttp && isValidUrl.Scheme != Uri.UriSchemeHttps)
                return false;

            if (_httpAccount.IsBasicAuthentication)
            {
                // when authentication is set then we need a user name
                if (string.IsNullOrWhiteSpace(Username))
                    return false;

                if (!AskForPasswordLater)
                {
                    if (string.IsNullOrWhiteSpace(Password))
                        return false;
                }
            }

            return true;
        }

        protected override void HandleInteractionObjectChanged()
        {
            _httpAccount = Interaction.HttpAccount;

            RaisePropertyChanged(nameof(Url));
            RaisePropertyChanged(nameof(Username));
            RaisePropertyChanged(nameof(Password));
            AskForPasswordLater = string.IsNullOrWhiteSpace(Password);
            RaisePropertyChanged(nameof(AskForPasswordLater));
            HasBasicAuthentication = _httpAccount.IsBasicAuthentication;
            RaisePropertyChanged(nameof(HasBasicAuthentication));
            RaisePropertyChanged(nameof(Timeout));
            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
