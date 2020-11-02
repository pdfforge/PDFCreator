using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Printer;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Collections.Generic;
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
            var profiles = ProfilesProvider.Settings.Select(x => new ConversionProfileWrapper(x));
            if (PrinterMappings == null)
                _printerMappings = new Presentation.Helper.SynchronizedCollection<PrinterMappingWrapper>(new List<PrinterMappingWrapper>());
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator", ""), profiles));
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator2", ""), profiles));
            PdfCreatorPrinters = new List<string>{string.Empty};
            PrimaryPrinter = PdfCreatorPrinters.First();
        }
    }
}
