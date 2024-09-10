using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Windows.Feedback;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeFeedbackWindowViewModel : FeedbackWindowViewModel
    {
        public DesignTimeFeedbackWindowViewModel() : 
            base(new DesignTimeTranslationUpdater(), new DesignTimeFeedbackSender(), new DesignTimeInteractionInvoker(), new DesignTimeOpenFileInteractionHelper())
        {
        }
    }
}
