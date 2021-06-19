using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Print;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePrintUserControlViewModel : PrintUserControlViewModel
    {
        public DesignTimePrintUserControlViewModel() : base(new DesignTimeActionLocator(), null,
            new DesignTimeTranslationUpdater(), new DesignTimeErrorCodeInterpreter(),
            new DesignTimeCurrentSettingsProvider(),
            new DesignTimeDispatcher(),
            new DesignTimeDefaultSettingsBuilder(),
            new DesignTimeActionOrderHelper(false, false),
            new DesignTimeCurrentSettings<ObservableCollection<PrinterMapping>>())
        {
        }
    }
}
