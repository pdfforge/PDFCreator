using Optional;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background
{
    public class BackgroundUserControlViewModel : ProfileUserControlViewModel<BackgroundSettingsAndActionTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IPdfVersionHelper _pdfVersionHelper;
        public TokenReplacer TokenReplacer { get; private set; }

        public TokenViewModel<ConversionProfile> BackgroundTokenViewModel { get; set; }

        public BackgroundUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater,
                                              ISelectedProfileProvider selectedProfile, ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory,
                                              IDispatcher dispatcher, IPdfVersionHelper pdfVersionHelper)
            : base(translationUpdater, selectedProfile, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;
            _pdfVersionHelper = pdfVersionHelper;
        }

        public override void MountView()
        {
            if (_tokenHelper != null)
            {
                TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForExternalFiles();
                BackgroundTokenViewModel = _tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.BackgroundPage.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectBackgroundAction)
                    .Build();
                RaisePropertyChanged(nameof(BackgroundTokenViewModel));
                BackgroundTokenViewModel.MountView();
            }

            CheckIfVersionIsPdf20();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            BackgroundTokenViewModel.UnmountView();
        }

        private Option<string> SelectBackgroundAction(string s1)
        {
            var titel = Translation.SelectBackgroundFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.BackgroundPage.File, titel, filter);
            interactionResult.MatchSome(s =>
            {
                BackgroundTokenViewModel.Text = s;
                BackgroundTokenViewModel.RaiseTextChanged();
            });

            CheckIfVersionIsPdf20();

            return interactionResult;
        }

        private void CheckIfVersionIsPdf20()
        {
            IsPdf20 = _pdfVersionHelper.CheckIfVersionIsPdf20(CurrentProfile.BackgroundPage.File);

            RaisePropertyChanged(nameof(IsPdf20));
        }

        public bool IsPdf20 { get; set; }
    }
}
