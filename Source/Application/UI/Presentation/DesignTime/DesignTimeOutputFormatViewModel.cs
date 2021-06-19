using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeOutputFormatViewModel : OutputFormatViewModel
    {
        public DesignTimeOutputFormatViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
        }
    }

    internal class DesignTimeOutputFormatPngViewModel : OutputFormatViewModel
    {
        public DesignTimeOutputFormatPngViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Png;
        }
    }

    internal class DesignTimeOutputFormatJpgViewModel : OutputFormatJpgViewModel
    {
        public DesignTimeOutputFormatJpgViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Jpeg;
        }
    }

    internal class DesignTimeOutputFormatTiffViewModel : OutputFormatViewModel
    {
        public DesignTimeOutputFormatTiffViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Tif;
        }
    }

    public class DesignTimeOutputFormatTextViewModel : OutputFormatViewModel
    {
        public DesignTimeOutputFormatTextViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Txt;
        }
    }
}
