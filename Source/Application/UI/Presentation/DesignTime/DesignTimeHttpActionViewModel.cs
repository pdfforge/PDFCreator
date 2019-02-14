using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHttpActionViewModel : HttpActionViewModel
    {
        public DesignTimeHttpActionViewModel() : base(new DesignTimeTranslationUpdater(), null, new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator(), null, new GpoSettingsDefaults())
        {
        }
    }
}
