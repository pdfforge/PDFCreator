using System.Drawing;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Font;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    internal class DesignTimeFontSelectorControlViewModel : FontSelectorControlViewModel
    {
        public DesignTimeFontSelectorControlViewModel() :base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeDispatcher(), new FontHelper(), null, 
            profile => profile.Stamping.FontName, profile => profile.Stamping.FontFile, 
            profile => profile.Stamping.FontSize, profile => profile.Stamping.Color) {
        }
    }
}
