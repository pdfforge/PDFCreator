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
        public DesignTimePrinterViewModel() : base(
            new DefaultSettingsProvider(),
            new DesignTimeCurrentSettings<ObservableCollection<PrinterMapping>>(),
            new DesignTimeCurrentSettings<ObservableCollection<ConversionProfile>>(),
            null,
            new DesignTimeTranslationUpdater(),
            new DesignTimePrinterHelper(),
            new GpoSettingsDefaults(),
            new DesignTimeInteractionRequest()
            )

        {
            PrinterMappings = new Presentation.Helper.SynchronizedCollection<PrinterMappingWrapper>(new List<PrinterMappingWrapper>()).ObservableCollection;
            var profiles = new List<ConversionProfileWrapper>() { new ConversionProfileWrapper(new ConversionProfile())};
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator", ""), profiles));
            PrinterMappings.Add(new PrinterMappingWrapper(new PrinterMapping("PDFCreator2", ""), profiles));
            PrimaryPrinter = PrinterMappings.First().PrinterName;
        }
    }
}
