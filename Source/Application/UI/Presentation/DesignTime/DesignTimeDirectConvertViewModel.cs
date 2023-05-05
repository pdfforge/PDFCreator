using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeDirectConvertViewModel : DirectConvertViewModel
    {
        public DesignTimeDirectConvertViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettings<ApplicationSettings>())
        {
        }
    }
}
