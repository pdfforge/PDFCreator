using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SmtpPasswordStepTranslation : PasswordButtonControlTranslation
    {
        public string SmtpPasswordOverlayTitle { get; private set; } = "SMTP Mail";
        public string SmtpAccountLabel { get; private set; } = "SMTP Account:";
        public string SmtpServerPasswordLabel { get; private set; } = "SMTP Server Password:";
    }
}
