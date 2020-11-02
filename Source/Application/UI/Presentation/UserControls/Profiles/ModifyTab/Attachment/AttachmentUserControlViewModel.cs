using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public class AttachmentUserControlViewModel : ProfileUserControlViewModel<AttachmentSettingsAndActionTranslation>
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly IPdfVersionHelper _pdfVersionHelper;
        private readonly ISelectFilesUserControlViewModelFactory _selectFilesUserControlViewModelFactory;

        public TokenReplacer TokenReplacer { get; private set; }

        public SelectFilesUserControlViewModel AttachmentFileSelectFilesUserControlViewModel { get; set; }

        public AttachmentUserControlViewModel(ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfile, ITokenHelper tokenHelper,
            IDispatcher dispatcher, IPdfVersionHelper pdfVersionHelper,
            ISelectFilesUserControlViewModelFactory selectFilesUserControlViewModelFactory)
            : base(translationUpdater, selectedProfile, dispatcher)
        {
            _tokenHelper = tokenHelper;
            _pdfVersionHelper = pdfVersionHelper;
            _selectFilesUserControlViewModelFactory = selectFilesUserControlViewModelFactory;
        }

        public override void MountView()
        {
            if (_tokenHelper != null)
            {
                TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForExternalFiles();
                var filter = Translation.PDFFiles
                             + @" (*.pdf)|*.pdf|"
                             + Translation.AllFiles
                             + @" (*.*)|*.*";

                AttachmentFileSelectFilesUserControlViewModel = _selectFilesUserControlViewModelFactory.Builder()
                    .WithTitleGetter(() => Translation.SelectAttachmentFile)
                    .WithFileListGetter(profile => profile.AttachmentPage.Files)
                    .WithFileFilter(filter)
                    .WithTokens(tokens)
                    .Build();
                AttachmentFileSelectFilesUserControlViewModel.PropertyChanged += (s, a) =>
                {
                    if (a.PropertyName == nameof(AttachmentFileSelectFilesUserControlViewModel.FileListDictionary))
                    {
                        CheckIfVersionIsPdf20();
                    }
                };

                RaisePropertyChanged(nameof(AttachmentFileSelectFilesUserControlViewModel));
                AttachmentFileSelectFilesUserControlViewModel.MountView();
            }

            CheckIfVersionIsPdf20();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            AttachmentFileSelectFilesUserControlViewModel.UnmountView();
        }

        private void CheckIfVersionIsPdf20()
        {
            Pdf20Files = CurrentProfile.AttachmentPage.Files.FindAll(_pdfVersionHelper.CheckIfVersionIsPdf20);

            RaisePropertyChanged(nameof(IsAnyPdf20));
            RaisePropertyChanged(nameof(Pdf20Files));
        }

        public bool IsAnyPdf20 => Pdf20Files?.Any() ?? false;

        public List<string> Pdf20Files
        {
            get;
            set;
        }
    }

    public class DesignTimeAttachmentUserControlViewModel : AttachmentUserControlViewModel
    {
        public DesignTimeAttachmentUserControlViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(),
                                                        null, null, new PdfVersionHelper(), new DesignTimeSelectFilesUserControlViewModelFactory())
        {
        }
    }
}
