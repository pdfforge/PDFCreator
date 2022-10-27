using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public class DesignTimeRssFeedViewModel : RssFeedViewModel
    {
        public DesignTimeRssFeedViewModel() : base(
            new DesignTimeCommandLocator(),
            new DesignTimeCurrentSettings<Conversion.Settings.RssFeed>(),
            new GpoSettingsDefaults(),
            new DesignTimeTranslationUpdater(),
            null, null, null, null, null,
            new DesignTimeVersionHelper(), new DesignTimeApplicationNameProvider())
        { }
    }
}
