using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAddActionOverlayViewModel : AddActionOverlayViewModel
    {
        public DesignTimeAddActionOverlayViewModel() :
            base(new DesignTimeEventAggregator(), new DesignTimeCurrentSettingsProvider(),
                new List<IPresenterActionFacade>(), new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator())
        {
        }
    }
}
