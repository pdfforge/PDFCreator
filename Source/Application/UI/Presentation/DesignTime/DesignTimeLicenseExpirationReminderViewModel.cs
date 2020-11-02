using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeLicenseExpirationReminderViewModel : LicenseExpirationReminderViewModel
    {
        public DesignTimeLicenseExpirationReminderViewModel() : base(new DesignTimeLicenseExpirationReminder(), new DesignTimeCommandLocator(), new DesignTimeTranslationUpdater(), null)
        {
        }
    }
}
