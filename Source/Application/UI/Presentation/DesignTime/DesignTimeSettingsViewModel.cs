using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSettingsViewModel : SettingsViewModel
    {
        public DesignTimeSettingsViewModel() : base(null, new DesignTimeTranslationUpdater(), new EventAggregator(), new DesignTimeCommandLocator(), new DesignTimeEditionHelper())
        {
        }
    }
}
