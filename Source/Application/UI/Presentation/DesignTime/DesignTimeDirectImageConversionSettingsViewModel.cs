using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeDirectImageConversionSettingsViewModel : DirectImageConversionSettingsViewModel
    {
        public DesignTimeDirectImageConversionSettingsViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()))
        {
        }
    }
}
