using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeDefaultViewerViewModel : DefaultViewerViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettingsProvider = new DesignTimeCurrentSettingsProvider();

        public DesignTimeDefaultViewerViewModel() : base(new DesignTimeTranslationUpdater(), null, CurrentSettingsProvider, null, null)
        {
        }
    }
}
