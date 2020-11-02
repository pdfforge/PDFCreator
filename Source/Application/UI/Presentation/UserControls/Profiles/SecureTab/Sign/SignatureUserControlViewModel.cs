using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Converter;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using SystemInterface.IO;
using IInteractionRequest = pdfforge.Obsidian.Trigger.IInteractionRequest;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public class SignatureUserControlViewModel : ProfileUserControlViewModel<SignUserControlTranslation>, IMountable
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountsProvider;
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
        private readonly IFile _file;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IGpoSettings _gpoSettings;
        private readonly ISigningPositionToUnitConverterFactory _signingPositionToUnitConverter;
        private readonly IHashUtil _hashUtil;
        private readonly IInteractionRequest _interactionRequest;
        public ICurrentSettings<ApplicationSettings> ApplicationSettings { get; }

        private ISigningPositionToUnitConverter UnitConverter { get; set; }

        private (string signatureHash, bool isValid) _lastSignatureCheck = ("Not empty", false);

        public float LeftX
        {
            get
            {
                if (CurrentProfile?.PdfSettings?.Signature == null)
                    return 0f;

                return UnitConverter.ConvertBack(Signature.LeftX);
            }
            set
            {
                var width = UnitConverter.ConvertToUnit(SignatureHeight);
                Signature.LeftX = UnitConverter.ConvertToUnit(value);
                Signature.RightX = Signature.LeftX + width; // adapt right position to maintain width
            }
        }

        public float LeftY
        {
            get
            {
                if (CurrentProfile?.PdfSettings?.Signature == null)
                    return 0f;

                return UnitConverter.ConvertBack(Signature.LeftY);
            }
            set
            {
                var height = UnitConverter.ConvertToUnit(SignatureHeight);
                Signature.LeftY = UnitConverter.ConvertToUnit(value);
                Signature.RightY = Signature.LeftY + height; // adapt right position to maintain height
            }
        }

        public float SignatureWidth
        {
            get
            {
                if (CurrentProfile?.PdfSettings?.Signature == null)
                    return 0f;

                return UnitConverter.ConvertBack(Signature.RightX - Signature.LeftX);
            }
            set
            {
                Signature.RightX = UnitConverter.ConvertToUnit(value) + Signature.LeftX;
            }
        }

        public float SignatureHeight
        {
            get
            {
                if (CurrentProfile?.PdfSettings?.Signature == null)
                    return 0f;

                return UnitConverter.ConvertBack(Signature.RightY - Signature.LeftY);
            }
            set
            {
                Signature.RightY = UnitConverter.ConvertToUnit(value) + Signature.LeftY;
            }
        }

        public TokenViewModel<ConversionProfile> SignReasonTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignContactTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignLocationTokenViewModel { get; private set; }

        public ICollectionView TimeServerAccountsView { get; set; }

        private ObservableCollection<TimeServerAccount> _timeServerAccounts;

        public IMacroCommand EditTimeServerAccountCommand { get; set; }
        public IMacroCommand AddTimeServerAccountCommand { get; set; }
        public Signature Signature => CurrentProfile?.PdfSettings.Signature;
        public DelegateCommand ChooseCertificateFileCommand { get; private set; }
        public DelegateCommand ChangeUnitConverterCommand { get; private set; }
        public AsyncCommand SignaturePasswordCommand { get; private set; }

        public string Password
        {
            get { return Signature?.SignaturePassword; }
            set
            {
                if (Signature.SignaturePassword != value)
                {
                    Signature.SignaturePassword = value;
                    RaisePropertyChanged(nameof(PasswordHint));
                }
            }
        }

        public string CertificateFile
        {
            get { return Signature?.CertificateFile; }
            set
            {
                if (Signature.CertificateFile != value)
                {
                    Signature.CertificateFile = value;
                    RaisePropertyChanged(nameof(CertificateFile));
                    RaisePropertyChanged(nameof(PasswordHint));
                    SignaturePasswordCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        private bool _allowConversionInterrupts = true;

        public bool AllowConversionInterrupts
        {
            private get
            {
                return _allowConversionInterrupts;
            }

            set
            {
                _allowConversionInterrupts = value;
                AskForPasswordLater &= _allowConversionInterrupts;
            }
        }

        public SignatureUserControlViewModel(
            IOpenFileInteractionHelper openFileInteractionHelper,
            ICurrentSettings<Conversion.Settings.Accounts> accountsProvider, ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator, ISignaturePasswordCheck signaturePasswordCheck,
            IFile file, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher,
            IGpoSettings gpoSettings, ISigningPositionToUnitConverterFactory signingPositionToUnitConverter,
            ICurrentSettings<ApplicationSettings> applicationSettings,
            IHashUtil hashUtil, IInteractionRequest interactionRequest)
        : base(translationUpdater, currentSettingsProvider, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _accountsProvider = accountsProvider;
            _translationUpdater = translationUpdater;
            _currentSettingsProvider = currentSettingsProvider;

            _signaturePasswordCheck = signaturePasswordCheck;
            _file = file;
            _tokenViewModelFactory = tokenViewModelFactory;
            _gpoSettings = gpoSettings;

            _signingPositionToUnitConverter = signingPositionToUnitConverter;
            _hashUtil = hashUtil;
            _interactionRequest = interactionRequest;
            ApplicationSettings = applicationSettings;
            UnitConverter = _signingPositionToUnitConverter?.CreateSigningPositionToUnitConverter(UnitOfMeasurement.Centimeter);

            ChooseCertificateFileCommand = new DelegateCommand(ChooseCertificateFileExecute);
            ChangeUnitConverterCommand = new DelegateCommand(ChangeUnitConverterExecute);

            AddTimeServerAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditTimeServerAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();

            SignaturePasswordCommand = new AsyncCommand(SignaturePasswordCommandExecute, SignaturePasswordCommandCanExecute);
        }

        private bool _wasInit;

        public override void MountView()
        {
            if (!_wasInit)
            {
                _translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(_tokenViewModelFactory));
                _wasInit = true;
            }

            _currentSettingsProvider.SelectedProfileChanged += OnCurrentSettingsProviderOnSelectedProfileChanged;

            _timeServerAccounts = _accountsProvider?.Settings.TimeServerAccounts;
            if (_timeServerAccounts != null)
            {
                TimeServerAccountsView = new ListCollectionView(_timeServerAccounts);
                TimeServerAccountsView.SortDescriptions.Add(new SortDescription(nameof(TimeServerAccount.AccountInfo), ListSortDirection.Ascending));
            }

            if (Signature != null)
                AskForPasswordLater = string.IsNullOrEmpty(Password);

            SignReasonTokenViewModel.MountView();
            SignContactTokenViewModel.MountView();
            SignLocationTokenViewModel.MountView();
            EditTimeServerAccountCommand.MountView();

            base.MountView();
        }

        private void OnCurrentSettingsProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            SetTokenViewModels(_tokenViewModelFactory);
        }

        public override void UnmountView()
        {
            base.UnmountView();

            _currentSettingsProvider.SelectedProfileChanged -= OnCurrentSettingsProviderOnSelectedProfileChanged;
            SignReasonTokenViewModel?.UnmountView();
            SignContactTokenViewModel?.UnmountView();
            SignLocationTokenViewModel?.UnmountView();
            EditTimeServerAccountCommand.UnmountView();
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListWithFormatting());

            SignReasonTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.SignReason)
                .Build();

            SignContactTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.SignContact)
                .Build();

            SignLocationTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.SignLocation)
                .Build();

            RaisePropertyChanged(nameof(SignReasonTokenViewModel));
            RaisePropertyChanged(nameof(SignContactTokenViewModel));
            RaisePropertyChanged(nameof(SignLocationTokenViewModel));
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _timeServerAccounts.Last();
            TimeServerAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            TimeServerAccountsView.Refresh();
        }

        private void ChangeUnitConverterExecute(object obj)
        {
            // values must be saved in local variables before the converter is changed
            // so that we can maintain the real coordinates of the signature position
            var unit = (UnitOfMeasurement)obj;
            UnitConverter = _signingPositionToUnitConverter.CreateSigningPositionToUnitConverter(unit);

            RaisePropertyChanged(nameof(LeftX));
            RaisePropertyChanged(nameof(LeftY));
            RaisePropertyChanged(nameof(SignatureWidth));
            RaisePropertyChanged(nameof(SignatureHeight));
        }

        private void ChooseCertificateFileExecute(object obj)
        {
            var title = Translation.SelectCertFile;
            var filter = Translation.PfxP12Files
                         + @" (*.pfx, *.p12)|*.pfx;*.p12|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(Signature.CertificateFile, title, filter);

            interactionResult.MatchSome(s =>
            {
                CertificateFile = s;
                RaisePropertyChanged(nameof(Signature));
            });
        }

        private bool SignaturePasswordCommandCanExecute(object o)
        {
            if (string.IsNullOrWhiteSpace(CertificateFile))
                return false;

            if (!_file.Exists(CertificateFile))
                return false;

            return true;
        }

        private async Task SignaturePasswordCommandExecute(object obj)
        {
            var interaction =
                new SignaturePasswordInteraction(PasswordMiddleButton.None, CertificateFile) { Password = Password };

            await _interactionRequest.RaiseAsync(interaction);

            switch (interaction.Result)
            {
                case PasswordResult.StorePassword:
                    Password = interaction.Password;
                    break;

                case PasswordResult.RemovePassword:
                    Password = "";
                    break;
            }
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(TimeServerAccountsView));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(CertificateFile));
            RaisePropertyChanged(nameof(PasswordHint));
            RaisePropertyChanged(nameof(AskForPasswordLater));

            RaisePropertyChanged(nameof(LeftX));
            RaisePropertyChanged(nameof(LeftY));
            RaisePropertyChanged(nameof(SignatureWidth));
            RaisePropertyChanged(nameof(SignatureHeight));

            if (CurrentProfile.AutoSave.Enabled)
                AskForPasswordLater = false;
            else if (string.IsNullOrEmpty(Password))
                AskForPasswordLater = true;
        }

        private bool _askForPasswordLater;

        public bool AskForPasswordLater
        {
            get { return _askForPasswordLater; }
            set
            {
                _askForPasswordLater = value && AllowConversionInterrupts;
                if (_askForPasswordLater)
                {
                    Password = "";
                    RaisePropertyChanged(nameof(Password));
                }
                RaisePropertyChanged(nameof(AskForPasswordLater));
                RaisePropertyChanged(nameof(PasswordHint));
            }
        }

        /// <summary>
        /// PasswordHint returns an empty string if password is ok
        /// </summary>
        public string PasswordHint
        {
            get
            {
                if (string.IsNullOrEmpty(CertificateFile))
                    return "";

                if (!_file.Exists(CertificateFile))
                    return Translation.CertificateDoesNotExist;

                if (CurrentProfile.AutoSave.Enabled || !AllowConversionInterrupts)
                    if (string.IsNullOrWhiteSpace(Password))
                        return Translation.AutosaveRequiresPasswords;

                if (!CurrentProfile.AutoSave.Enabled && AskForPasswordLater && AllowConversionInterrupts)
                    return "";

                var signatureHash = _hashUtil.GetSha1Hash(Signature.CertificateFile + "|" + Password);
                if (_lastSignatureCheck.signatureHash != signatureHash)
                {
                    var isValid = _signaturePasswordCheck.IsValidPassword(Signature.CertificateFile, Password);
                    _lastSignatureCheck = (signatureHash, isValid);
                }

                if (_lastSignatureCheck.isValid)
                    return "";

                return Translation.WrongPassword;
            }
        }
    }
}
