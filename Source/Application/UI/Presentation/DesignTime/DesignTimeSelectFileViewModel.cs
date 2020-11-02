using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSelectFileViewModel : SelectFileViewModel
    {
        public DesignTimeSelectFileViewModel() : base(
            new DesignTimeTranslationUpdater(),
            new DesignTimeTokenViewModelFactory(),
            null)
        {
        }
    }
}
