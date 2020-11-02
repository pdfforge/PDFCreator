using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.ForwardToOtherProfile;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeForwardToFurtherProfileViewModel : ForwardToFurtherProfileViewModel
    {
        public DesignTimeForwardToFurtherProfileViewModel()
            : base(new DesignTimeTranslationUpdater(),
                   new DesignTimeCurrentSettingsProvider(),
                   new DispatcherWrapper(),
                   new DesignTimeCurrentSettings<ObservableCollection<ConversionProfile>>(),
                   new EditionHelper(false))
        {
        }
    }
}
