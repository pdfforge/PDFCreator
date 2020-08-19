using Optional;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class FtpAccountViewModel : AccountViewModelBase<FtpAccountInteraction, AccountsTranslation>, IMountable
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        public FtpAccount FtpAccount { get; set; }
        public TokenViewModel<FtpAccount> FtpAccountTokenViewModel { get; set; }
        public TokenReplacer TokenReplacer { get; private set; }

        public FtpAccountViewModel(ITranslationUpdater translationUpdater, IOpenFileInteractionHelper openFileInteractionHelper,
            ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory) : base(translationUpdater)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;
        }

        private Option<string> SelectPrivateKeyFile(string arg)
        {
            var titel = Translation.SelectKeyFile;
            var filter = Translation.KeyFiles
                         + @" (*.ppk;*.key)|*.ppk;*.key|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(FtpAccount.PrivateKeyFile, titel, filter);
            interactionResult.MatchSome(s =>
            {
                FtpAccountTokenViewModel.Text = s;
                FtpAccountTokenViewModel.RaiseTextChanged();
                SaveCommand.RaiseCanExecuteChanged();
            });

            return interactionResult;
        }

        public string ProtocolPrefix => IsSftpConnection ? "sftp://" : "ftp://";

        public bool IsSftpConnection => FtpConnectionType == FtpConnectionType.Sftp;

        public string Password
        {
            get => FtpAccount?.Password;
            set { FtpAccount.Password = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string Server
        {
            get => FtpAccount?.Server;
            set
            {
                FtpAccount.Server = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public FtpConnectionType FtpConnectionType
        {
            get => FtpAccount?.FtpConnectionType ?? FtpConnectionType.Ftp;
            set
            {
                FtpAccount.FtpConnectionType = value;
                if (FtpAccount.FtpConnectionType == FtpConnectionType.Ftp)
                    NormalAuthentication = true;

                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(IsSftpConnection));
                RaisePropertyChanged(nameof(ProtocolPrefix));
            }
        }

        public string UserName
        {
            get => FtpAccount?.UserName;
            set
            {
                FtpAccount.UserName = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool NormalAuthentication
        {
            get => FtpAccount?.AuthenticationType == AuthenticationType.NormalAuthentication;
            set
            {
                if (value)
                {
                    FtpAccount.AuthenticationType = AuthenticationType.NormalAuthentication;
                    RaisePropertyChangedForAuthenticationTypeProperties();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool KeyFileAuthentication
        {
            get => FtpAccount?.AuthenticationType == AuthenticationType.KeyFileAuthentication;
            set
            {
                if (value)
                {
                    FtpAccount.AuthenticationType = AuthenticationType.KeyFileAuthentication;
                    RaisePropertyChangedForAuthenticationTypeProperties();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowPassForKeyFile => NormalAuthentication || KeyFileRequiresPass;

        public bool KeyFileRequiresPass

        {
            get => FtpAccount?.KeyFileRequiresPass ?? false;
            set
            {
                FtpAccount.KeyFileRequiresPass = value;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChangedForAuthenticationTypeProperties();
            }
        }

        protected override bool SaveCanExecute()
        {
            return CheckAllRequiredData();
        }

        private bool CheckAllRequiredData()
        {
            if (KeyFileAuthentication)
            {
                return (KeyFileRequiresPass && (AskForPasswordLater || !string.IsNullOrWhiteSpace(Password)))
                       || !KeyFileRequiresPass
                       && !string.IsNullOrWhiteSpace(FtpAccount.PrivateKeyFile)
                       && !string.IsNullOrWhiteSpace(Server)
                       && !string.IsNullOrWhiteSpace(UserName);
            }

            return ((AskForPasswordLater || !string.IsNullOrWhiteSpace(Password))
                                            && !string.IsNullOrWhiteSpace(Server)
                                            && !string.IsNullOrWhiteSpace(UserName));
        }

        protected override void SaveExecute()
        {
            Interaction.FtpAccount = FtpAccount;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            FtpAccount = Interaction.FtpAccount;

            RaisePropertyChanged(nameof(Server));
            RaisePropertyChanged(nameof(UserName));
            RaisePropertyChanged(nameof(Password));

            if (FtpAccount.AuthenticationType == AuthenticationType.KeyFileAuthentication)
                AskForPasswordLater = string.IsNullOrWhiteSpace(Password) && KeyFileRequiresPass;
            else
                AskForPasswordLater = string.IsNullOrWhiteSpace(Password);

            RaisePropertyChanged(nameof(AskForPasswordLater));
            RaisePropertyChanged(nameof(IsSftpConnection));
            RaisePropertyChanged(nameof(ProtocolPrefix));
            SaveCommand.RaiseCanExecuteChanged();
        }

        protected override void ClearPassword()
        {
            FtpAccount.Password = "";
        }

        public void MountView()
        {
            if (_tokenHelper != null)
            {
                TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForExternalFiles();

                FtpAccountTokenViewModel = _tokenViewModelFactory.Builder<FtpAccount>()
                    .WithInitialValue(FtpAccount)
                    .WithSelector(p => p.PrivateKeyFile)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectPrivateKeyFile)
                    .Build();
                RaisePropertyChanged(nameof(FtpAccountTokenViewModel));

                FtpAccountTokenViewModel.MountView();
            }

            RaisePropertyChanged(nameof(FtpAccount));
            RaisePropertyChanged(nameof(FtpConnectionType));
            RaisePropertyChangedForAuthenticationTypeProperties();
            SaveCommand.RaiseCanExecuteChanged();
        }

        public void UnmountView()
        {
            FtpAccountTokenViewModel.UnmountView();
        }

        private void RaisePropertyChangedForAuthenticationTypeProperties()
        {
            RaisePropertyChanged(nameof(NormalAuthentication));
            RaisePropertyChanged(nameof(KeyFileAuthentication));
            RaisePropertyChanged(nameof(KeyFileRequiresPass));
            RaisePropertyChanged(nameof(ShowPassForKeyFile));
        }
    }
}
