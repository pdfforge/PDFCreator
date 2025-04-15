using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class HttpAccountViewModel : AccountViewModelBase<HttpAccountInteraction, HttpTranslation>
    {
        private HttpAccount _httpAccount;
        private readonly IHttpAction _httpAction;
        public IList<HttpSendMode> HttpSendModes => new List<HttpSendMode> { HttpSendMode.HttpPost, HttpSendMode.HttpWebDav };

        public HttpAccountViewModel(ITranslationUpdater translationUpdater,
            ITokenViewModelFactory tokenViewModelFactory, IHttpAction httpAction) : base(translationUpdater)
        {
            UrlTokenViewModel = tokenViewModelFactory
                .Builder<HttpAccount>()
                .WithSelector(account => account.Url)
                .WithTokenList(th => th.GetTokenListWithFormatting())
                .WithDefaultTokenReplacerPreview()
                .Build();
            UrlTokenViewModel.TextChanged += (s, a) => { SaveCommand.RaiseCanExecuteChanged(); };

            _httpAction = httpAction;
        }

        public TokenViewModel<HttpAccount> UrlTokenViewModel { get; set; }

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
            }
        }

        public string Password
        {
            get { return _httpAccount?.Password; }
            set
            {
                _httpAccount.Password = value;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(Password));
            }
        }

        public HttpSendMode SendMode
        {
            get { return _httpAccount?.SendMode ?? HttpSendMode.HttpPost; }
            set
            {
                if (_httpAccount == null)
                    return;
                _httpAccount.SendMode = value;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(SendMode));
            }
        }

        public bool IsBasicAuthentication
        {
            get { return _httpAccount != null && _httpAccount.IsBasicAuthentication; }
            set
            {
                _httpAccount.IsBasicAuthentication = value;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(IsBasicAuthentication));
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
            var userTokenEnabled = true; //Ignore the UserTokenRequiredCheck
            var result = _httpAction.CheckAccount(_httpAccount, !AskForPasswordLater, userTokenEnabled, CheckLevel.EditingProfile);
            return result.IsSuccess;
        }

        protected override void HandleInteractionObjectChanged()
        {
            _httpAccount = Interaction.HttpAccount;

            UrlTokenViewModel.CurrentValue = _httpAccount;
            RaisePropertyChanged(nameof(Timeout));
            RaisePropertyChanged(nameof(IsBasicAuthentication));
            RaisePropertyChanged(nameof(Username));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(SendMode));
            AskForPasswordLater = string.IsNullOrWhiteSpace(Password);
            RaisePropertyChanged(nameof(AskForPasswordLater));
            SaveCommand.RaiseCanExecuteChanged();
        }

        protected override void ClearPassword()
        {
            _httpAccount.Password = "";
        }
    }
}
