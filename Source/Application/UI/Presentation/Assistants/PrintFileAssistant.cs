using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    /// <summary>
    ///     The PrintFileAssistant extends the PrintFileHelperBase with user interaction (ask to change default printer, show
    ///     responses for invalid files).
    /// </summary>
    public class PrintFileAssistant : PrintFileHelperBase
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private PrintFilesTranslation _translation;

        public PrintFileAssistant(IInteractionInvoker interactionInvoker, IPrinterHelper printerHelper, ISettingsProvider settingsProvider, ITranslationUpdater translationUpdater, IFileAssoc fileAssoc, IDirectory directory, IFile file)
            : base(printerHelper, settingsProvider, fileAssoc, directory, file)
        {
            _interactionInvoker = interactionInvoker;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
        }

        protected override void DirectoriesNotSupportedHint()
        {
            const string caption = "PDFCreator";
            var message = _translation.DirectoriesNotSupported;
            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Warning);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }

        private string BuildUnprintableFilesMessage(IList<PrintCommand> unprintable)
        {
            var fileList =
                new List<string>(unprintable.Select(p => Path.GetFileName(p.Filename)).Take(Math.Min(3, unprintable.Count)));
            
            var message = _translation.GetNotPrintableFiles(unprintable.Count) + System.Environment.NewLine;

            message += string.Join("\r\n", fileList.ToArray());

            if (fileList.Count < unprintable.Count)
                message += "\r\n" + _translation.GetAndXMoreMessage(unprintable.Count - fileList.Count);

            return message;
        }

        protected override void UnprintableFilesHint(IList<PrintCommand> unprintable)
        {
            const string caption = "PDFCreator";
            var message = BuildUnprintableFilesMessage(unprintable);

            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Warning);
        }

        protected override bool UnprintableFilesProceedQuery(IList<PrintCommand> unprintable)
        {
            const string caption = "PDFCreator";
            var message = BuildUnprintableFilesMessage(unprintable);

            message += "\r\n\r\n" + _translation.ProceedAnyway;

            var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Warning);

            return response == MessageResponse.Yes;
        }

        protected override bool QuerySwitchDefaultPrinter()
        {
            var message =
                _translation.AskSwitchDefaultPrinter;
            const string caption = "PDFCreator";

            var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Question);

            return response == MessageResponse.Yes;
        }
    }
}
