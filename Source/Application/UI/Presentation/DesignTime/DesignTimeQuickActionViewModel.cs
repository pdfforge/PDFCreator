using System.Collections.ObjectModel;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeQuickActionViewModel : QuickActionViewModel
    {
        public DesignTimeQuickActionViewModel() : base(
            new DesignTimeTranslationUpdater(), 
            new DesignTimeCommandLocator(),
            new ReadableReadableFileSizeFormatter(), 
            new DesignTimeCurrentSettings<ObservableCollection<ConversionProfile>>(), 
            new DesignTimeCurrentSettingsProvider(), 
            new DesignTimeAttachToOutlookItemAssistant())
        { }
    }
}
