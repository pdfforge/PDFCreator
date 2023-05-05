using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    public class ApplicationSettingsViewTranslation:ITranslatable
    {
        public string Debug { get; private set; } = "Debug";
        public string General { get; private set; } = "General";
        public string Title { get; private set; } = "Title";
        public string Viewer { get; private set; } = "Viewer";
        public string License { get; private set; } = "License";
        public string DirectImageConversion { get; private set; } = "Direct Convert";

    }
}
