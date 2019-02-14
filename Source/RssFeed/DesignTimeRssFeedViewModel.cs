using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public class DesignTimeRssFeedViewModel : RssFeedViewModel
    {
        public DesignTimeRssFeedViewModel() : base(
            new DesignTimeCommandLocator(),
            new DesignTimeCurrentSettings<Conversion.Settings.RssFeed>(),
            null,
            new DesignTimeTranslationUpdater(),
            null, null)
        { }
    }
}
