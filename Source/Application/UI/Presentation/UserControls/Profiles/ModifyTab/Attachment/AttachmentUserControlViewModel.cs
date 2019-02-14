using Optional;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public class AttachmentUserControlViewModel : ProfileUserControlViewModel<AttachmentSettingsAndActionTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public TokenReplacer TokenReplacer { get; }

        public TokenViewModel<ConversionProfile> AttachmentFileTokenViewModel { get; set; }

        public AttachmentUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater,
                                              ISelectedProfileProvider selectedProfile,
            ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher)
            : base(translationUpdater, selectedProfile, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = tokenHelper.GetTokenListForExternalFiles();

                AttachmentFileTokenViewModel = tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.AttachmentPage.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectAttatchmentAction)
                    .Build();

                RaisePropertyChanged(nameof(AttachmentFileTokenViewModel));
            }
        }

        private Option<string> SelectAttatchmentAction(string s1)
        {
            var title = Translation.SelectAttachmentFile;
            var filter = Translation.PDFFiles
                         + @" (*.pdf)|*.pdf|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.AttachmentPage.File, title, filter);

            interactionResult.MatchSome(s =>
            {
                AttachmentFileTokenViewModel.Text = s;
                AttachmentFileTokenViewModel.RaiseTextChanged();
            });

            return interactionResult;
        }
    }

    public class DesignTimeAttachmentUserControlViewModel : AttachmentUserControlViewModel
    {
        public DesignTimeAttachmentUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, null, null)
        {
        }
    }
}
