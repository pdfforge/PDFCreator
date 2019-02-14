using Optional;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover
{
    public class CoverUserControlViewModel : ProfileUserControlViewModel<CoverSettingsTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public TokenReplacer TokenReplacer { get; }

        public TokenViewModel<ConversionProfile> CoverPageTokenViewModel { get; set; }

        public CoverUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfile, ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher) : base(translationUpdater, selectedProfile, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = tokenHelper.GetTokenListForExternalFiles();

                CoverPageTokenViewModel = tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.CoverPage.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectCoverPageAction)
                    .Build();

                RaisePropertyChanged(nameof(CoverPageTokenViewModel));
            }
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

            return interactionResult;
        }
    }

    public class DesignTimeCoverUserControlViewModel : CoverUserControlViewModel
    {
        public DesignTimeCoverUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, null, null)
        {
        }
    }
}
