using Optional;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public class AttachmentUserControlViewModel : ProfileUserControlViewModel<AttachmentSettingsAndActionTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly EditionHelper _editionHelper;
        private readonly IPdfVersionHelper _pdfVersionHelper;

        public TokenReplacer TokenReplacer { get; private set; }

        public TokenViewModel<ConversionProfile> AttachmentFileTokenViewModel { get; set; }

        public AttachmentUserControlViewModel(IOpenFileInteractionHelper openFileInteractionHelper, ITranslationUpdater translationUpdater,
                                              ISelectedProfileProvider selectedProfile,
                                              ITokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory,
                                              IDispatcher dispatcher, EditionHelper editionHelper, IPdfVersionHelper pdfVersionHelper)
            : base(translationUpdater, selectedProfile, dispatcher)
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

                AttachmentFileTokenViewModel = _tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.AttachmentPage.File)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectAttatchmentAction)
                    .Build();

                RaisePropertyChanged(nameof(AttachmentFileTokenViewModel));
                AttachmentFileTokenViewModel.MountView();
            }

            CheckIfVersionIsPdf20();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            AttachmentFileTokenViewModel.UnmountView();
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

            CheckIfVersionIsPdf20();

            return interactionResult;
        }

        private void CheckIfVersionIsPdf20()
        {
            IsPdf20 = _pdfVersionHelper.CheckIfVersionIsPdf20(CurrentProfile.AttachmentPage.File);

            RaisePropertyChanged(nameof(IsPdf20));
        }

        public bool IsPdf20 { get; set; }
    }

    public class DesignTimeAttachmentUserControlViewModel : AttachmentUserControlViewModel
    {
        public DesignTimeAttachmentUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(),
                                                        null, null, null, new EditionHelper(false), new PdfVersionHelper())
        {
        }
    }
}
