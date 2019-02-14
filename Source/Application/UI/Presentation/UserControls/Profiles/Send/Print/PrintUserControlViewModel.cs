using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print
{
    public class PrintUserControlViewModel : ProfileUserControlViewModel<PrintTabTranslation>
    {
        public PrintUserControlViewModel(ISystemPrinterProvider systemPrinterProvider, ITranslationUpdater translationUpdater, ISelectedProfileProvider profile, IDispatcher dispatcher) : base(translationUpdater, profile, dispatcher)
        {
            _systemPrinterProvider = systemPrinterProvider;
        }

        private readonly ISystemPrinterProvider _systemPrinterProvider;

        // ReSharper disable once MemberCanBePrivate.Global
        public bool PrinterDialogOptionEnabled { get; set; } = true;

        public IEnumerable<string> InstalledPrinters => _systemPrinterProvider.GetInstalledPrinterNames();

        public EnumTranslation<SelectPrinter>[] SelectPrinterValues => PrinterDialogOptionEnabled ?
            Translation.SelectPrinterValues :
            (EnumTranslation<SelectPrinter>[])Translation.SelectPrinterValues.Where(x => x.Value != SelectPrinter.ShowDialog).ToArray();
    }

    public class DesignTimePrintUserControlViewModel : PrintUserControlViewModel
    {
        public DesignTimePrintUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
        }
    }
}
