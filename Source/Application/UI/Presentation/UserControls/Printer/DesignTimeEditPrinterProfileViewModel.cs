using System.Collections.ObjectModel;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class DesignTimeEditPrinterProfileViewModel : EditPrinterProfileViewModel
    {
        public DesignTimeEditPrinterProfileViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
