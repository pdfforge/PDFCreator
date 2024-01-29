using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class AttachToOutlookItemTranslation : ITranslatable
    {
        public string AttachToOutlookItem { get; set; } = "Attach to Outlook item";
        public string ItemCouldNotBeFound { get; set; } = "The selected Outlook item could not be found:";
        public string ErrorWhileAddingAttachment { get; set; } = "Error while adding attachment to selected Outlook item:";
        public string AttachTo { get; set; } = "Attach to:";
        public string Skip { get; set; } = "Skip";
    }
}
