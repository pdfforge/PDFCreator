﻿using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Setup.Shared.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Web;
using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class TerminalServerNotAllowedCondition : IStartupCondition
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly ITerminalServerDetection _terminalServerDetection;
        private readonly ProgramTranslation _translation;

        public bool CanRequestUserInteraction => true;

        public TerminalServerNotAllowedCondition(ITerminalServerDetection terminalServerDetection, ITranslationFactory translationFactory,
            IInteractionInvoker interactionInvoker, IWebLinkLauncher webLinkLauncher, ApplicationNameProvider applicationNameProvider)
        {
            _terminalServerDetection = terminalServerDetection;
            _translation = translationFactory.CreateTranslation<ProgramTranslation>();
            _interactionInvoker = interactionInvoker;
            _webLinkLauncher = webLinkLauncher;
            _applicationNameProvider = applicationNameProvider;
        }

        public StartupConditionResult Check()
        {
            if (!_terminalServerDetection.IsTerminalServer() || _terminalServerDetection.IsWindowsEnterpriseMultiSession())
                return StartupConditionResult.BuildSuccess();

            var errorMessage = $"It is not possible to run {_applicationNameProvider.ApplicationName} with installed Terminal Services. Please use the Terminal Server Edition instead.";
            _logger.Error(errorMessage);
            var caption = _applicationNameProvider.ApplicationName;
            var message = _translation.UsePDFCreatorTerminalServer;
            var result = ShowMessage(message, caption, MessageOptions.MoreInfoCancel, MessageIcon.Exclamation);
            if (result == MessageResponse.MoreInfo)
                _webLinkLauncher.Launch(Urls.PdfCreatorTerminalServerUrl);

            return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.NotValidOnTerminalServer, errorMessage, showMessage: false);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, options, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
