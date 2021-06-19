using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile
{
    public class OpenViewerActionViewModel : ActionViewModelBase<DefaultViewerAction, OpenViewerActionTranslation>
    {
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

        public OpenViewerActionViewModel(ITranslationUpdater translationUpdater,
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IDispatcher dispatcher,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
        }

        protected override string SettingsPreviewString
        {
            get
            {
                if (CurrentProfile.OpenViewer.OpenWithPdfArchitect)
                {
                    return Translation.OpenWithPdfArchitect;
                }

                return Translation.OpenWithDefault;
            }
        }
    }
}
