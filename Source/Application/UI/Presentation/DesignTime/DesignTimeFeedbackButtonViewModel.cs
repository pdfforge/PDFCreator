using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeFeedbackButtonViewModel : FeedbackButtonViewModel
    {
        public DesignTimeFeedbackButtonViewModel() : base(new DesignTimeCommandLocator(), new DesignTimeTranslationUpdater(),new GpoSettingsDefaults())
        { }
    }
}
