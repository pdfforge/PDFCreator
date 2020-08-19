using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class SaveTabViewModel : ProfileUserControlViewModel<SaveTabTranslation>, ITabViewModel
    {
        private readonly ITokenButtonFunctionProvider _buttonFunctionProvider;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;

        public string Title => Translation.SaveTabText;
        public IconList Icon => IconList.SaveSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public bool HasNotSupportedFeatures => false;

        private TokenReplacer _tokenReplacer = new TokenReplacer();
        public TokenViewModel<ConversionProfile> FileNameViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> FolderViewModel { get; private set; }
        public bool AllowSkipPrintDialog { get; }

        public SaveTabViewModel(ITokenButtonFunctionProvider buttonFunctionProvider, ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater, EditionHelper editionHelper, ITokenHelper tokenHelper,
            ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            AllowSkipPrintDialog = !editionHelper.IsFreeEdition;
            _buttonFunctionProvider = buttonFunctionProvider;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;

            translationUpdater?.RegisterAndSetTranslation(tf => SetTokenViewModels());

            CurrentProfile?.MountRaiseConditionsForNotSupportedFeatureSections((s, a) =>
            {
                RaisePropertyChanged(nameof(HasNotSupportedSettingsForSavingTempOnly));
            });
        }

        private void SetTokenViewModels()
        {
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
            var builder = _tokenViewModelFactory
                .BuilderWithSelectedProfile();

            FileNameViewModel = builder
                .WithTokenList(_tokenHelper.GetTokenListForFilename())
                .WithTokenCustomPreview(PreviewForFileName)
                .WithSelector(p => p.FileNameTemplate)
                .Build();

            FolderViewModel = builder
                    .WithTokenList(_tokenHelper.GetTokenListForDirectory())
                    .WithTokenCustomPreview(PreviewForFolder)
                    .WithSelector(p => p.TargetDirectory)
                    .WithButtonCommand(_buttonFunctionProvider.GetBrowseFolderFunction(Translation.ChooseFolder))
                    .Build();

            RaisePropertyChanged(nameof(FileNameViewModel));
            RaisePropertyChanged(nameof(FolderViewModel));
            RaisePropertyChanged(nameof(HasNotSupportedSettingsForSavingTempOnly));
        }

        public override void MountView()
        {
            FileNameViewModel.MountView();
            FolderViewModel.MountView();
            RaisePropertyChanged(nameof(FileNameViewModel));
            RaisePropertyChanged(nameof(FolderViewModel));
            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            FileNameViewModel.UnmountView();
            FolderViewModel.UnmountView();
        }

        private string PreviewForFileName(string s)
        {
            return ValidName.MakeValidFileName(_tokenReplacer.ReplaceTokens(s));
        }

        private string PreviewForFolder(string s)
        {
            return ValidName.MakeValidFolderName(_tokenReplacer.ReplaceTokens(s));
        }

        public bool HasNotSupportedSettingsForSavingTempOnly
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.HasNotSupportedSendSettings();
            }
        }

        public bool TemporarySaveFiles
        {
            get => CurrentProfile != null ? CurrentProfile.SaveFileTemporary : false;
            set
            {
                CurrentProfile.SaveFileTemporary = value;
                RaisePropertyChanged(nameof(HasNotSupportedSettingsForSavingTempOnly));
            }
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(TemporarySaveFiles));
        }
    }
}
