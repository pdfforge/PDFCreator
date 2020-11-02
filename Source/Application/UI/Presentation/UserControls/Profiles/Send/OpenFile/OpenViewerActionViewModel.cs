using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.OpenFile
{
    public class OpenViewerActionViewModel : ProfileUserControlViewModel<OpenViewerActionTranslation>
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

        public OpenViewerActionViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider, IDispatcher dispatcher) : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
        }
    }

    public class DesignTimeOpenViewerActionViewModel : OpenViewerActionViewModel
    {
        public DesignTimeOpenViewerActionViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
        }
    }
}
