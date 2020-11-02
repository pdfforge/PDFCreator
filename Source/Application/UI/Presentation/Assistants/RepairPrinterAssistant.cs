using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IRepairPrinterAssistant
    {
        bool TryRepairPrinter(IEnumerable<string> printerNames);

        bool IsRepairRequired();
    }

    public class RepairPrinterAssistant : IRepairPrinterAssistant
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IPDFCreatorNameProvider _nameProvider;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IPrinterHelper _printerHelper;
        private readonly ApplicationTranslation _translation;
        private readonly IShellExecuteHelper _shellExecuteHelper;
        private readonly IFile _file;

        public RepairPrinterAssistant(IInteractionInvoker interactionInvoker, IPrinterHelper printerHelper, IShellExecuteHelper shellExecuteHelper, IFile file, IAssemblyHelper assemblyHelper, IPDFCreatorNameProvider nameProvider, ITranslationFactory iTranslationFactory)
        {
            _interactionInvoker = interactionInvoker;
            _printerHelper = printerHelper;
            _translation = iTranslationFactory.CreateTranslation<ApplicationTranslation>(); ;
            _shellExecuteHelper = shellExecuteHelper;
            _file = file;
            _assemblyHelper = assemblyHelper;
            _nameProvider = nameProvider;
        }

        public string DefaultPrinterName { get; set; } = "PDFCreator";

        public bool TryRepairPrinter(IEnumerable<string> printerNames)
        {
            Logger.Error("It looks like the printers are broken. This needs to be fixed to allow PDFCreator to work properly");

            var title = _translation.RepairPrinterNoPrintersInstalled;
            var message = _translation.RepairPrinterAskUserUac;

            Logger.Debug("Asking to start repair..");

            var response = ShowMessage(message, title, MessageOptions.YesNo, MessageIcon.Exclamation);
            if (response == MessageResponse.Yes)
            {
                var applicationPath = _assemblyHelper.GetAssemblyDirectory();
                var printerHelperPath = PathSafe.Combine(applicationPath, "PrinterHelper.exe");

                if (!_file.Exists(printerHelperPath))
                {
                    Logger.Error("PrinterHelper.exe does not exist!");
                    title = _translation.Error;
                    message = _translation.GetSetupFileMissingMessage(PathSafe.GetFileName(printerHelperPath));

                    ShowMessage(message, title, MessageOptions.OK, MessageIcon.Error);
                    return false;
                }

                Logger.Debug("Reinstalling Printers...");
                var pdfcreatorPath = _nameProvider.GetPortApplicationPath();

                var printerNameString = GetPrinterNameString(printerNames);

                var installParams = $"RepairPrinter -name={printerNameString} -PortApplication=\"{pdfcreatorPath}\"";
                var installResult = _shellExecuteHelper.RunAsAdmin(printerHelperPath, installParams);
                Logger.Debug("Done: {0}", installResult);
            }

            Logger.Debug("Now we'll check again, if the printer is installed");
            if (IsRepairRequired())
            {
                Logger.Warn("The printer could not be repaired.");
                return false;
            }

            Logger.Info("The printer was repaired successfully");

            return true;
        }

        public bool IsRepairRequired()
        {
            return !_printerHelper.GetPDFCreatorPrinters().Any();
        }

        private string GetPrinterNameString(IEnumerable<string> printerNames)
        {
            var printers = printerNames.ToList();

            if (!printers.Any())
                printers.Add(DefaultPrinterName);

            return string.Join(",", printers.Select(printerName => $"\"{printerName}\""));
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
