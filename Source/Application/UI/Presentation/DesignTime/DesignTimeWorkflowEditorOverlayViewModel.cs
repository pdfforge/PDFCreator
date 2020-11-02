using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeWorkflowEditorOverlayViewModel : WorkflowEditorOverlayViewModel
    {
        public DesignTimeWorkflowEditorOverlayViewModel() : base(new Prism.Regions.RegionManager(), new DesignTimeTranslationUpdater())
        {
        }
    }
}
