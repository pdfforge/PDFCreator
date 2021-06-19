using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class PrinterInstalledCondition : IStartupCondition
    {
        private readonly IRepairPrinterAssistant _repairPrinterAssistant;
        private readonly ISettingsLoader _settingsLoader;
        private readonly ApplicationTranslation _translation;

        public bool CanRequestUserInteraction => false;

        public PrinterInstalledCondition(IRepairPrinterAssistant repairPrinterAssistant, ISettingsLoader settingsLoader, ITranslationFactory translationFactory)
        {
            _repairPrinterAssistant = repairPrinterAssistant;
            _settingsLoader = settingsLoader;
            _translation = translationFactory.CreateTranslation<ApplicationTranslation>();
        }

        public StartupConditionResult Check()
        {
            if (!_repairPrinterAssistant.IsRepairRequired())
                return StartupConditionResult.BuildSuccess();

            var printers = LoadPrinterNames();
            if (!_repairPrinterAssistant.TryRepairPrinter(printers))
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.PrintersBroken, _translation.RepairPrinterFailed);
            return StartupConditionResult.BuildSuccess();
        }

        private IEnumerable<string> LoadPrinterNames()
        {
            var settings = _settingsLoader.LoadPdfCreatorSettings();
            var printerMappings = settings.ApplicationSettings.PrinterMappings;
            return printerMappings.Select(mapping => mapping.PrinterName);
        }
    }
}
