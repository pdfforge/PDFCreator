using Optional;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover
{
    public class CoverUserControlViewModel : ProfileUserControlViewModel<CoverSettingsTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly EditionHelper _editionHelper;
        private readonly IPdfVersionHelper _pdfVersionHelper;

        public TokenReplacer TokenReplacer { get; private set; }

        public TokenViewModel<ConversionProfile> CoverPageTokenViewModel { get; set; }

        public CoverUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfile, ITokenHelper tokenHelper,
            ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher,
            EditionHelper editionHelper, IPdfVersionHelper pdfVersionHelper) : base(translationUpdater, selectedProfile, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;
            _editionHelper = editionHelper;
            _pdfVersionHelper = pdfVersionHelper;
        }

        public override void MountView()
        {
            if (_tokenHelper != null)
            {
                TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForExternalFiles();

                CoverPageTokenViewModel = _tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.CoverPage.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectCoverPageAction)
                    .Build();

                RaisePropertyChanged(nameof(CoverPageTokenViewModel));
                CoverPageTokenViewModel.MountView();
            }

            CheckIfVersionIsPdf20();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            CoverPageTokenViewModel.UnmountView();
        }

        private Option<string> SelectCoverPageAction(string s1)
        {
            var title = Translation.SelectCoverFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.CoverPage.File, title, filter);

            interactionResult.MatchSome(s =>
            {
                CoverPageTokenViewModel.Text = s;
                CoverPageTokenViewModel.RaiseTextChanged();
            });

            CheckIfVersionIsPdf20();

            return interactionResult;
        }

        private void CheckIfVersionIsPdf20()
        {
            IsPdf20 = _pdfVersionHelper.CheckIfVersionIsPdf20(CurrentProfile.CoverPage.File);

            RaisePropertyChanged(nameof(IsPdf20));
        }

        public bool IsPdf20 { get; set; }
    }

    public class DesignTimeCoverUserControlViewModel : CoverUserControlViewModel
    {
        public DesignTimeCoverUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(),
                                                null, null, null,
                                                new EditionHelper(false), new PdfVersionHelper())
        {
        }
    }
}
