using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeTitleReplacementsViewModel : TitleReplacementsViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettingsProvider = new DesignTimeCurrentSettingsProvider();

        public DesignTimeTitleReplacementsViewModel() : base(new DesignTimeTranslationUpdater(), null, CurrentSettingsProvider, new DesignTimeCommandLocator(), null)
        {
        }
    }
}
