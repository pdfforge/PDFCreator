using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.ProfessionalHintStep;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeProfessionalHintStepViewModel : ProfessionalHintStepViewModel
    {
        public DesignTimeProfessionalHintStepViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator(), new DesignTimeProfessionalHintHelper())
        {
        }
    }
}
