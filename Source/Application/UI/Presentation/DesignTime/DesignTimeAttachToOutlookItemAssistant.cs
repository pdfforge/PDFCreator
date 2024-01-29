using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAttachToOutlookItemAssistant : AttachToOutlookItemAssistant
    {
        public DesignTimeAttachToOutlookItemAssistant() 
            : base(new DesignTimeTranslationUpdater(), new DesignTimeAttachToOutlookItem(), new DesignTimeInteractionRequest())
        {  }
    }
}
