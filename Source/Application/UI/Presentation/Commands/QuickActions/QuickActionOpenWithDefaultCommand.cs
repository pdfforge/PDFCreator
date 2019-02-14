using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionOpenWithDefaultCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        protected readonly IDefaultViewerAction Action;
        protected readonly IFileAssoc FileAssoc;
        protected readonly ICommandLocator CommandLocator;
        protected readonly ISettingsProvider SettingsProvider;
        protected readonly IInteractionInvoker InteractionInvoker;

        public QuickActionOpenWithDefaultCommand(ITranslationUpdater translationUpdater, IDefaultViewerAction action, IFileAssoc fileAssoc, ICommandLocator commandLocator, ISettingsProvider settingsProvider, IInteractionInvoker interactionInvoker) : base(translationUpdater)

        {
            Action = action;
            FileAssoc = fileAssoc;
            CommandLocator = commandLocator;
            SettingsProvider = settingsProvider;
            InteractionInvoker = interactionInvoker;
        }

        public override void Execute(object obj)
        {
            var path = GetPaths(obj).FirstOrDefault();

            if (!path.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                HandleActionResult(Action.OpenOutputFile(path));
            }
            else
            {
                if (FileAssoc.HasOpen(".pdf") || SettingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf).IsActive)
                {
                    HandleActionResult(Action.OpenOutputFile(path));
                }
                else
                {
                    CommandLocator.GetCommand<QuickActionOpenWithPdfArchitectCommand>().Execute(obj);
                }
            }
        }

        protected void HandleActionResult(ActionResult result)
        {
            if (result.Contains(ErrorCode.DefaultViewer_Not_Found))
            {
                ShowMessage(Translation.ErrorCustomViewNotFoundDesc, Translation.ErrorCustomViewNotFoundTitle, MessageOptions.OK, MessageIcon.Error);
            }
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, options, icon);
            InteractionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
