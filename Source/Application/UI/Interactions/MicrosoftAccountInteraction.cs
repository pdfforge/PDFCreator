using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class MicrosoftAccountInteraction:AccountInteractionBase
    {
        public MicrosoftAccount MicrosoftAccount { get; set; }

        public MicrosoftAccountInteraction(MicrosoftAccount microsoftAccount, string title)
        {
            MicrosoftAccount = microsoftAccount;
            Title = title;
        }
    }
}
