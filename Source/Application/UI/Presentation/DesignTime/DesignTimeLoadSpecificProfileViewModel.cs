using System.Collections.ObjectModel;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeLoadSpecificProfileViewModel : LoadSpecificProfileViewModel
    {
        public DesignTimeLoadSpecificProfileViewModel() : base(
            null, 
            null, 
            new DesignTimeCommandLocator(),
            new DesignTimeCurrentSettings<ObservableCollection<ConversionProfile>>(),
            new DesignTimeCurrentSettings<ApplicationSettings>(),
            null,
            new DesignTimeTranslationUpdater())
        { }
    }
}
