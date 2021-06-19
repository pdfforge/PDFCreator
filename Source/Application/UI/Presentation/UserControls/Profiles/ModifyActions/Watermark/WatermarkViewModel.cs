using Optional;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions
{
    public class WatermarkViewModel : ActionViewModelBase<WatermarkAction, WatermarkTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IPdfVersionHelper _pdfVersionHelper;
        public TokenReplacer TokenReplacer { get; private set; }

        public TokenViewModel<ConversionProfile> WatermarkTokenViewModel { get; set; }

        public WatermarkViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater, ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory,
                                              IDispatcher dispatcher, IPdfVersionHelper pdfVersionHelper, ICurrentSettingsProvider currentSettingsProvider,
                                              IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter, IDefaultSettingsBuilder defaultSettingsBuilder,
                                              IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;
            _pdfVersionHelper = pdfVersionHelper;
        }

        protected override string SettingsPreviewString => CurrentProfile?.Watermark.File;

        public override void MountView()
        {
            if (_tokenHelper != null)
            {
                TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForExternalFiles();
                WatermarkTokenViewModel = _tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.Watermark.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectWatermarkFileAction)
                    .Build();
                RaisePropertyChanged(nameof(WatermarkTokenViewModel));
                WatermarkTokenViewModel.MountView();
            }

            CheckIfVersionIsPdf20();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            WatermarkTokenViewModel.UnmountView();
        }

        private Option<string> SelectWatermarkFileAction(string s1)
        {
            var titel = Translation.SelectWatermarkFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.BackgroundPage.File, titel, filter);
            interactionResult.MatchSome(s =>
            {
                WatermarkTokenViewModel.Text = s;
                WatermarkTokenViewModel.RaiseTextChanged();
            });

            CheckIfVersionIsPdf20();

            return interactionResult;
        }

        private void CheckIfVersionIsPdf20()
        {
            IsPdf20 = _pdfVersionHelper.CheckIfVersionIsPdf20(CurrentProfile.Watermark.File);

            RaisePropertyChanged(nameof(IsPdf20));
        }

        public bool IsPdf20 { get; set; }
    }
}
