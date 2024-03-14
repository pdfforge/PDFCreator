using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IPrinterAssistant
    {
        Task<string> AddPrinter();

        Task<bool> DeletePrinter(string printerName, int numPrinters);

        Task<string> RenamePrinter(string oldPrinterName);
    }

    public class PrinterAssistant : TranslatableViewModelBase<PrinterAssistantTranslation>, IPrinterAssistant
    {
        private readonly IPrinterHelper _printerHelper;

        private readonly IUacAssistant _uacAssistant;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IOsHelper _osHelper;

        public PrinterAssistant(IPrinterHelper printerHelper, IUacAssistant uacAssistant, ITranslationUpdater translationUpdater, IInteractionRequest interactionRequest,
                                       IOsHelper osHelper)
            : base(translationUpdater)
        {
            _printerHelper = printerHelper;
            _uacAssistant = uacAssistant;
            _interactionRequest = interactionRequest;
            _osHelper = osHelper;
        }

        public async Task<string> AddPrinter()
        {
            var newPrinterName = _printerHelper.CreateValidPrinterName("PDFCreator");
            var questionText = Translation.EnterPrintername;
            newPrinterName = await RequestPrinternameFromUser(questionText, newPrinterName);
            if (newPrinterName == null)
                return null;

            while (!_printerHelper.IsValidPrinterName(newPrinterName))
            {
                questionText = Translation.GetPrinterAlreadyInstalledMessage(newPrinterName);
                newPrinterName = _printerHelper.CreateValidPrinterName(newPrinterName);
                newPrinterName = await RequestPrinternameFromUser(questionText, newPrinterName);
                if (newPrinterName == null)
                    return null;
            }

            if (await _uacAssistant.AddPrinter(newPrinterName))
                if (CheckInstalledPrinter(newPrinterName))
                    return newPrinterName;

            const string caption = "PDFCreator";
            var message = Translation.GetCouldNotInstallPrinterMessage(newPrinterName);
            await ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);

            return null;
        }

        public async Task<string> RenamePrinter(string oldPrinterName)
        {
            string newPrinterName = null;

            if (oldPrinterName.Length == 0)
                return null;

            var questionText = Translation.EnterPrintername;
            newPrinterName = await RequestPrinternameFromUser(questionText, oldPrinterName);

            if ((newPrinterName == null) || (newPrinterName == oldPrinterName))
                return null;

            while (!_printerHelper.IsValidPrinterName(newPrinterName))
            {
                questionText = Translation.GetPrinterAlreadyInstalledMessage(newPrinterName);
                newPrinterName = _printerHelper.CreateValidPrinterName(newPrinterName);
                newPrinterName = await RequestPrinternameFromUser(questionText, newPrinterName);
                if ((newPrinterName == null) || (newPrinterName == oldPrinterName))
                    return null;
            }

            if (_uacAssistant.RenamePrinter(oldPrinterName, newPrinterName))
                if (CheckInstalledPrinter(newPrinterName))
                    return newPrinterName;

            var message = Translation.GetCouldNotRenamePrinterMessage(oldPrinterName, newPrinterName);
            const string caption = @"PDFCreator";
            await ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);

            return null;
        }

        public async Task<bool> DeletePrinter(string printerName, int numPrinters)
        {
            if (numPrinters < 2)
            {
                var message = Translation.DontDeleteLastPrinter;
                const string caption = @"PDFCreator";
                await ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);
                return false;
            }

            var msg = Translation.GetAskDeletePrinterMessage(printerName);
            var cpt = Translation.DeletePrinter;

            var messageOptions = _osHelper.UserIsAdministrator() ? MessageOptions.YesNo
                : MessageOptions.YesNoUac;

            var resonse = await ShowMessage(msg, cpt, messageOptions, MessageIcon.Question);

            if (resonse != MessageResponse.Yes)
                return false;

            return _uacAssistant.DeletePrinter(printerName);
        }

        private async Task<MessageResponse> ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);

            await _interactionRequest.RaiseAsync(interaction);
            return interaction.Response;
        }

        private bool CheckInstalledPrinter(string printerName)
        {
            var ps = new PrinterSettings();
            ps.PrinterName = printerName;
            return ps.IsValid;
        }

        private async Task<string> RequestPrinternameFromUser(string questionText, string printerName)
        {
            var showUacShield = !_osHelper.UserIsAdministrator();
            var interaction = new InputInteraction("PDFCreator", questionText, ValidatePrinterName, showUacShield);
            interaction.InputText = printerName;

            var i = await _interactionRequest.RaiseAsync(interaction);
            if (!i.Success)
                return null;

            return i.InputText.Trim();
        }

        private InputValidation ValidatePrinterName(string arg)
        {
            switch (_printerHelper.ValidatePrinterName(arg))
            {
                case PrinterNameValidation.Valid:
                    return new InputValidation(true, "");

                case PrinterNameValidation.InvalidName:
                    return new InputValidation(false, Translation.InvalidCharsInPrinterName);

                case PrinterNameValidation.AlreadyExists:
                default:
                    return new InputValidation(false, Translation.InvalidPrinterName);
            }
        }
    }
}
