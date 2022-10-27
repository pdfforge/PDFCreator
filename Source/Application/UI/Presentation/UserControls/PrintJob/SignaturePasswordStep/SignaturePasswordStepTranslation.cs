using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SignaturePasswordStepTranslation : PasswordButtonControlTranslation
    {
        public string SignaturePasswordTitle { get; private set; } = "Set Certificate Password";
        public string CertificatePassword { get; private set; } = "Certificate _Password:";
        public string CertificateFile { get; private set; } = "Certificate File:";
        public string IncorrectPassword { get; private set; } = "The entered password is not valid for this certificate";
    }
}
