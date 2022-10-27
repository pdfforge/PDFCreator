using pdfforge.PDFCreator.UI.Presentation.Helper.Font;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeFontSelectorControlViewModelFactory : IFontSelectorControlViewModelFactory
    {
        public IFontSelectorControlViewModelBuilder BuilderWithSelectedProfile()
        {
            return new FontSelectorControlViewModelBuilder(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, new FontHelper(), new DesignTimeDispatcher());
        }
    }
}
