using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Web;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile
{
    public class OpenViewerActionViewModel : ActionViewModelBase<OpenFileAction, OpenViewerActionTranslation>
    {
        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly IPdfEditorHelper _pdfEditorHelper;

        public bool UseDefaultViewer
        {
            get
            {
                if (CurrentProfile == null)
                    return false;

                return !CurrentProfile.OpenViewer.OpenWithPdfArchitect;
            }
            set
            {
                CurrentProfile.OpenViewer.OpenWithPdfArchitect = !value;
                RaisePropertyChanged(nameof(UseDefaultViewer));
            }
        }

        public string OpenWithViewerTranslation => _pdfEditorHelper.UseSodaPdf ? Translation.FormatOpenWithCustomViewer("Soda PDF") : Translation.OpenWithPdfArchitect;
        public string MoreInfoOnEditorTranslation => _pdfEditorHelper.UseSodaPdf ? Translation.FormatEditorMoreInfo("Soda PDF") : Translation.FormatEditorMoreInfo("PDF Architect");

        public OpenViewerActionViewModel(ITranslationUpdater translationUpdater,
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IDispatcher dispatcher,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper,
            IWebLinkLauncher webLinkLauncher,
            IPdfEditorHelper pdfEditorHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _webLinkLauncher = webLinkLauncher;
            PdfArchitectInfoCommand = new DelegateCommand(ExecutePdfArchitectInfoCommand);
            _pdfEditorHelper = pdfEditorHelper;
        }

        private void ExecutePdfArchitectInfoCommand(object obj)
        {
            var url = _pdfEditorHelper.UseSodaPdf
                ? Urls.SodaPdfWebsiteUrl
                : Urls.ArchitectWebsiteUrl;

            _webLinkLauncher.Launch(url);
        }

        public ICommand PdfArchitectInfoCommand { get; }

        protected override string SettingsPreviewString
        {
            get
            {
                if (CurrentProfile.OpenViewer.OpenWithPdfArchitect)
                {
                    return OpenWithViewerTranslation;
                }

                return Translation.OpenWithDefault;
            }
        }
    }
}
