using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeErrorViewModel : ErrorViewModel
    {
        public DesignTimeErrorViewModel()
            : base( new DesignTimeTranslationUpdater(), new TranslationFactory(null))
        {
            Error = new Error(MessageIcon.Error, "Here goes the title", "Here goes the Preface","This is the first Error Message\nAnd here we have a second line of error");
        }
    }
}
