using Optional;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Font;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using pdfforge.PDFCreator.Utilities;
using IInteractionRequest = pdfforge.Obsidian.Trigger.IInteractionRequest;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SignatureActionViewModel : ActionViewModelBase<SigningAction, SignatureTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IGpoSettings _gpoSettings;
        private readonly IInteractionRequest _interactionRequest;
        private readonly EditionHelper _editionHelper;
        private readonly ISignaturePositionAndSizeHelper _signaturePositionAndSizeHelper;
        public ICurrentSettings<ApplicationSettings> ApplicationSettings { get; }

        public TokenViewModel<ConversionProfile> SignReasonTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignContactTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignLocationTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> ImageFileTokenViewModel { get; private set; }
        public FontSelectorControlViewModel FontSelectorControlViewModel { get; private set; }

        public ICollectionView TimeServerAccountsView { get; set; }

        private ObservableCollection<TimeServerAccount> _timeServerAccounts;

        public IMacroCommand EditTimeServerAccountCommand { get; set; }
        public IMacroCommand AddTimeServerAccountCommand { get; set; }

        public DelegateCommand DrawSignatureCommand { get; set; }

        public Conversion.Settings.Signature Signature => CurrentProfile?.PdfSettings.Signature;
        public DelegateCommand ChooseCertificateFileCommand { get; private set; }
        public AsyncCommand EditPositionAndSizeCommand { get; private set; }
        public AsyncCommand SignaturePasswordCommand { get; private set; }

        public DisplaySignature DisplaySignature
        {
            get
            {
                return CurrentProfile == null ? DisplaySignature.NoDisplay : CurrentProfile.PdfSettings.Signature.DisplaySignature;
            }
            set
            {
                CurrentProfile.PdfSettings.Signature.DisplaySignature = value;
                RaisePropertyChanged(nameof(DisplaySignature));
            }
        }

        public string Password
        {
            get { return Signature?.SignaturePassword; }
            set
            {
                if (Signature.SignaturePassword != value)
                {
                    Signature.SignaturePassword = value;
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
                    SignaturePasswordCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool EditAccountsIsAllowedByGpo => !_gpoSettings.DisableAccountsTab;

        private bool _allowConversionInterrupts = true;

        public bool AllowConversionInterrupts
        {
            private get { return _allowConversionInterrupts; }

            set
            {
                _allowConversionInterrupts = value;
                AskForPasswordLater &= _allowConversionInterrupts;
            }
        }

        public bool SupportsImage => !_editionHelper.IsFreeEdition;

        public SignatureActionViewModel(
            IOpenFileInteractionHelper openFileInteractionHelper,
            ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings,
            ICurrentSettings<ApplicationSettings> applicationSettings,
            IInteractionRequest interactionRequest,
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper,
            EditionHelper editionHelper,
            IFontSelectorControlViewModelFactory fontSelectorControlViewModelFactory,
            ISignaturePositionAndSizeHelper signaturePositionAndSizeHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _translationUpdater = translationUpdater;
            _currentSettingsProvider = currentSettingsProvider;

            _tokenViewModelFactory = tokenViewModelFactory;
            _gpoSettings = gpoSettings;

            _interactionRequest = interactionRequest;
            _editionHelper = editionHelper;
            _signaturePositionAndSizeHelper = signaturePositionAndSizeHelper;
            ApplicationSettings = applicationSettings;

            ChooseCertificateFileCommand = new DelegateCommand(ChooseCertificateFileExecute);

            AddTimeServerAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditTimeServerAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .AddCommand(new DelegateCommand(o => StatusChanged()))
                .Build();

            DrawSignatureCommand = new DelegateCommand(DrawSignatureExecute);

            SignaturePasswordCommand = new AsyncCommand(SignaturePasswordCommandExecute, SignaturePasswordCommandCanExecute);

            _timeServerAccounts = _currentSettingsProvider.CheckSettings.Accounts.TimeServerAccounts;
            if (_timeServerAccounts != null)
            {
                TimeServerAccountsView = new ListCollectionView(_timeServerAccounts);
                TimeServerAccountsView.SortDescriptions.Add(new SortDescription(nameof(TimeServerAccount.AccountInfo), ListSortDirection.Ascending));
            }

            EditPositionAndSizeCommand = new AsyncCommand(EditPositionAndSizeExecute);

            FontSelectorControlViewModel = fontSelectorControlViewModelFactory.BuilderWithSelectedProfile()
                .WithFontNameSelector(p => p.PdfSettings.Signature.FontName)
                .WithFontFileSelector(p => p.PdfSettings.Signature.FontFile)
                .WithFontSizeSelector(p => p.PdfSettings.Signature.FontSize)
                .WithFontColorSelector(p => p.PdfSettings.Signature.FontColor)
                .Build();
        }

        private async Task EditPositionAndSizeExecute(object o)
        {
            var unitOfMeasurement = ApplicationSettings.Settings.UnitOfMeasurement;
            var interaction = new SignaturePositionAndSizeInteraction(Signature, unitOfMeasurement);

            await _interactionRequest.RaiseAsync(interaction);
            if (interaction.Success)
            {
                CurrentProfile.PdfSettings.Signature = interaction.Signature;
                ApplicationSettings.Settings.UnitOfMeasurement = interaction.UnitOfMeasurement;
                SignaturePositionAndSizeChanged();
            }
        }

        private bool _wasInit;

        protected override string SettingsPreviewString => CurrentProfile.PdfSettings.Signature.CertificateFile;

        public string SignaturePositionAndSizeText { get; private set; } = "";

        private void SignaturePositionAndSizeChanged()
        {
            if (ApplicationSettings?.Settings == null)
                return;
            if (Signature == null)
                return;

            var unit = ApplicationSettings.Settings.UnitOfMeasurement;
            SignaturePositionAndSizeText = _signaturePositionAndSizeHelper.GetSignaturePositionAndSizeText(Signature, unit, Translation); ;
            RaisePropertyChanged(nameof(SignaturePositionAndSizeText));
        }

        private void OnCurrentSettingsProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            SetTokenViewModels(_tokenViewModelFactory);
        }

        public override void MountView()
        {
            if (!_wasInit)
            {
                _translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(_tokenViewModelFactory));
                _wasInit = true;
            }

            _currentSettingsProvider.SelectedProfileChanged += OnCurrentSettingsProviderOnSelectedProfileChanged;

            if (Signature != null)
                AskForPasswordLater = string.IsNullOrEmpty(Password);

            SignReasonTokenViewModel.MountView();
            SignContactTokenViewModel.MountView();
            SignLocationTokenViewModel.MountView();
            ImageFileTokenViewModel.MountView();
            FontSelectorControlViewModel.MountView();
            EditTimeServerAccountCommand.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            _currentSettingsProvider.SelectedProfileChanged -= OnCurrentSettingsProviderOnSelectedProfileChanged;
            SignReasonTokenViewModel?.UnmountView();
            SignContactTokenViewModel?.UnmountView();
            SignLocationTokenViewModel?.UnmountView();
            ImageFileTokenViewModel?.UnmountView();
            FontSelectorControlViewModel?.UnmountView();
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

            ImageFileTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.BackgroundImageFile)
                .WithButtonCommand(SelectImageFile)
                .Build();

            RaisePropertyChanged(nameof(SignReasonTokenViewModel));
            RaisePropertyChanged(nameof(SignContactTokenViewModel));
            RaisePropertyChanged(nameof(SignLocationTokenViewModel));
            RaisePropertyChanged(nameof(ImageFileTokenViewModel));
        }

        private void DrawSignatureExecute(object parameter)
        {
            var interaction = new DrawSignatureInteraction(Translation.DrawCustomSignatureLabel);
            _interactionRequest.Raise(interaction, DrawSignatureCallback);
        }

        private void DrawSignatureCallback(DrawSignatureInteraction interaction)
        {
            if (!interaction.Success)
            {
                return;
            }

            ImageFileTokenViewModel.Text = interaction.SignatureFilePath;
            ImageFileTokenViewModel.RaiseTextChanged();
        }

        private Option<string> SelectImageFile(string s1)
        {
            var title = Translation.SelectSignatureImageFile;
            var filter = Translation.ImageFiles
                         + @" (*.bmp, *.jpg, *.gif, *.png, *.tif, *.tiff)|*.bmp;*.jpg;*.gif;*.png;*.tif;*.tiff|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.BackgroundPage.File, title, filter);
            interactionResult.MatchSome(s =>
            {
                ImageFileTokenViewModel.Text = s;
                ImageFileTokenViewModel.RaiseTextChanged();
            });

            return interactionResult;
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
            return !string.IsNullOrWhiteSpace(CertificateFile);
        }

        private async Task SignaturePasswordCommandExecute(object obj)
        {
            var interaction =
                new SignaturePasswordInteraction(CertificateFile) { Password = Password };

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

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            SignaturePositionAndSizeChanged();
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(DisplaySignature));
            RaisePropertyChanged(nameof(TimeServerAccountsView));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(CertificateFile));
            RaisePropertyChanged(nameof(AskForPasswordLater));

            SignaturePositionAndSizeChanged();

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
            }
        }
    }
}
