using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeTokenViewModelFactory : TokenViewModelFactory
    {
        public DesignTimeTokenViewModelFactory() : base(new DesignTimeCurrentSettingsProvider(), new DesignTimeTokenHelper())
        { }
    }
}
