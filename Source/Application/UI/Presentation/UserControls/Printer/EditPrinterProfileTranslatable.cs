using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class EditPrinterProfileTranslatable : ITranslatable
    {
        public string SelectProfile { get; private set; } = "Please select a profile:";
        public string EditPrinterProfileTitle { get; private set; } = "Edit profile";
        public string Ok { get; protected set; } = "Ok";
        public string Cancel { get; private set; } = "Cancel";
    }
}
