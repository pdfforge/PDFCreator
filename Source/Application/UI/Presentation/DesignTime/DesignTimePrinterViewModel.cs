using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Printer;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Collections.ObjectModel;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePrinterViewModel : PrinterViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettings = new DesignTimeCurrentSettingsProvider();

        public DesignTimePrinterViewModel() : base(
            new DesignTimePrinterProvider(),
            new DefaultSettingsProvider(),
            new DesignTimeCurrentSettings<ObservableCollection<PrinterMapping>>(),
            new DesignTimeCurrentSettings<ObservableCollection<ConversionProfile>>(),
            null,
            null,
            new DesignTimeTranslationUpdater(),
            new DesignTimePrinterHelper(),
            new GpoSettingsDefaults()
            )

        {
            var profiles = ProfilesProvider.Settings;

            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator", ""), profiles));
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator2", ""), profiles));
            PrimaryPrinter = PdfCreatorPrinters.First();
        }
    }
}
