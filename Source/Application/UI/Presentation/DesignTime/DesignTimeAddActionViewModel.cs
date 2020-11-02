using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAddActionViewModel : AddActionViewModel
    {
        public DesignTimeAddActionViewModel() : base(new DesignTimeCommandLocator(), new InteractionRequest(), new DesignTimeTranslationUpdater())
        {
        }
    }
}
