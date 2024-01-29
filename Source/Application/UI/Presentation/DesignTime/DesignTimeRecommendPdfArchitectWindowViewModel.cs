using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Reflection;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeRecommendPdfArchitectWindowViewModel : RecommendPdfArchitectWindowViewModel
    {
        public DesignTimeRecommendPdfArchitectWindowViewModel()
            : base(null, null, new DesignTimeTranslationUpdater(),
                new PdfArchitectCheck(new RegistryWrap(), new FileWrap(), new AssemblyHelper(Assembly.GetExecutingAssembly()), new ThreadManager()),
                new ProcessStarter(), new FileWrap(), new DesignTimeCurrentSettings<ApplicationSettings>(), new DesignTimeCurrentSettings<CreatorAppSettings>())

        {
        }
    }
}
