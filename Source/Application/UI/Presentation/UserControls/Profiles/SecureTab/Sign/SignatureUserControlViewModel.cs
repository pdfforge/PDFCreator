using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Converter;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public class SignatureUserControlViewModel : ProfileUserControlViewModel<SignUserControlTranslation>, IMountable
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ICommandLocator _commandLocator;
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
        private readonly IFile _file;
        private readonly IGpoSettings _gpoSettings;
        private readonly ISigningPositionToUnitConverterFactory _signingPositionToUnitConverter;
        public ICurrentSettings<ApplicationSettings> ApplicationSettings { get; }

        private ISigningPositionToUnitConverter UnitConverter { get; set; }

        private float _leftX;

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
                _leftX = value;
                Signature.LeftX = UnitConverter.ConvertToUnit(_leftX);
            }
        }

        private float _leftY;

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
                _leftY = value;
                Signature.LeftY = UnitConverter.ConvertToUnit(_leftY);
            }
        }

        private float _rightX;

        public float RightX
        {
            get
            {
                if (CurrentProfile?.PdfSettings?.Signature == null)
                    return 0f;

                return UnitConverter.ConvertBack(Signature.RightX - Signature.LeftX);
            }
            set
            {
                _rightX = value;
                Signature.RightX = UnitConverter.ConvertToUnit(_rightX + LeftX);
            }
        }

        private float _rightY;

        public float RightY
        {
            get
            {
                if (CurrentProfile?.PdfSettings?.Signature == null)
                    return 0f;

                return UnitConverter.ConvertBack(Signature.RightY - Signature.LeftY);
            }
            set
            {
                _rightY = value;
                Signature.RightY = UnitConverter.ConvertToUnit(_rightY + LeftY);
            }
        }

        public bool AllowConversionInterrupts
        {
            private get
            {
                return _allowConversionInterrupts;
            }

            set
            {
                _allowConversionInterrupts = value;
                AskForPasswordLater = false;
            }
        }

        public TokenViewModel<ConversionProfile> SignReasonTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignContactTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignLocationTokenViewModel { get; private set; }

        public ICollectionView TimeServerAccountsView { get; set; }

        private readonly ObservableCollection<TimeServerAccount> _timeServerAccounts;

        public IMacroCommand EditTimeServerAccountCommand { get; set; }
        public IMacroCommand AddTimeServerAccountCommand { get; set; }
        public Signature Signature => CurrentProfile?.PdfSettings.Signature;
        public DelegateCommand ChooseCertificateFileCommand { get; }
        public DelegateCommand ChangeUnitConverterCommand { get; }
        public bool OnlyForPlusAndBusiness { get; }

        public string Password
        {
            get { return Signature?.SignaturePassword; }
            set
            {
                Signature.SignaturePassword = value;
                RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            }
        }

        public string CertificateFile
        {
            get { return Signature?.CertificateFile; }
            set
            {
                Signature.CertificateFile = value;
                RaisePropertyChanged(nameof(CertificateFile));
                RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            }
        }

        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public SignatureUserControlViewModel(
            IOpenFileInteractionHelper openFileInteractionHelper, EditionHelper editionHelper,
            ICurrentSettings<Conversion.Settings.Accounts> accountsProvider, ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator, ISignaturePasswordCheck signaturePasswordCheck,
            IFile file, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher,
            IGpoSettings gpoSettings, ISigningPositionToUnitConverterFactory signingPositionToUnitConverter,
            ICurrentSettings<ApplicationSettings> applicationSettings)
        : base(translationUpdater, currentSettingsProvider, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _commandLocator = commandLocator;

            _signaturePasswordCheck = signaturePasswordCheck;
            _file = file;
            _gpoSettings = gpoSettings;

            _signingPositionToUnitConverter = signingPositionToUnitConverter;
            ApplicationSettings = applicationSettings;
            UnitConverter = _signingPositionToUnitConverter?.CreateSigningPositionToUnitConverter(UnitOfMeasurement.Centimeter);

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(tokenViewModelFactory));
            currentSettingsProvider.SelectedProfileChanged += (sender, args) => SetTokenViewModels(tokenViewModelFactory);

            if (editionHelper != null)
            {
                OnlyForPlusAndBusiness = editionHelper.ShowOnlyForPlusAndBusiness;
            }

            _timeServerAccounts = accountsProvider?.Settings.TimeServerAccounts;
            if (_timeServerAccounts != null)
            {
                TimeServerAccountsView = new ListCollectionView(_timeServerAccounts);
                TimeServerAccountsView.SortDescriptions.Add(new SortDescription(nameof(TimeServerAccount.AccountInfo), ListSortDirection.Ascending));
            }

            ChooseCertificateFileCommand = new DelegateCommand(ChooseCertificateFileExecute);

            ChangeUnitConverterCommand = new DelegateCommand(ChangeUnitConverterExecute);

            if (Signature != null)
                AskForPasswordLater = string.IsNullOrEmpty(Password);

            _timeServerAccountEditCommand = _commandLocator.GetCommand<TimeServerAccountEditCommand>();

            AddTimeServerAccountCommand = _commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditTimeServerAccountCommand = _commandLocator.CreateMacroCommand()
                .AddCommand(_timeServerAccountEditCommand)
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();
        }

        public void MountView()
        {
            ((IMountable)_timeServerAccountEditCommand).MountView();
        }

        public void UnmountView()
        {
            ((IMountable)_timeServerAccountEditCommand).UnmountView();
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
            var leftX = LeftX;
            var leftY = LeftY;
            var rightX = RightX;
            var rightY = RightY;

            var unit = (UnitOfMeasurement)obj;
            UnitConverter = _signingPositionToUnitConverter.CreateSigningPositionToUnitConverter(unit);

            LeftX = leftX;
            LeftY = leftY;
            RightX = rightX;
            RightY = rightY;
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

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(TimeServerAccountsView));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(CertificateFile));
            RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            RaisePropertyChanged(nameof(AskForPasswordLater));

            if (string.IsNullOrEmpty(Password))
                AskForPasswordLater = true;
        }

        public bool CertificatePasswordIsValid
        {
            get
            {
                if (string.IsNullOrEmpty(CertificateFile))
                    return false;

                if (!_file.Exists(CertificateFile))
                    return false;

                if (AskForPasswordLater)
                    return true;

                return _signaturePasswordCheck.IsValidPassword(Signature.CertificateFile, Password);
            }
        }

        private bool _askForPasswordLater;
        private bool _allowConversionInterrupts = true;
        private readonly ICommand _timeServerAccountEditCommand;

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
                RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            }
        }
    }
}
