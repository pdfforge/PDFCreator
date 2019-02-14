using Optional;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background
{
    public class BackgroundUserControlViewModel : ProfileUserControlViewModel<BackgroundSettingsAndActionTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        public TokenReplacer TokenReplacer { get; }

        public TokenViewModel<ConversionProfile> BackgroundTokenViewModel { get; set; }

        public BackgroundUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater,
                                              ISelectedProfileProvider selectedProfile, ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher)
            : base(translationUpdater, selectedProfile, null)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = tokenHelper.GetTokenListForExternalFiles();

                BackgroundTokenViewModel = tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.BackgroundPage.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectBackgroundAction)
                    .Build();

                RaisePropertyChanged(nameof(BackgroundTokenViewModel));
            }
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

            return interactionResult;
        }
    }

    public class DesignTimeBackgroundUserControlViewModel : BackgroundUserControlViewModel
    {
        public DesignTimeBackgroundUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, null, null)
        {
        }
    }
}
