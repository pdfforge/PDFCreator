using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.WorkflowQuery
{
    public class ErrorNotifierInteractive : IErrorNotifier
    {
        private readonly ITranslationFactory _translationFactory;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IInteractionRequest _interactionRequester;
        private InteractiveWorkflowTranslation _translation;

        public ErrorNotifierInteractive(ITranslationFactory translationFactory, IInteractionInvoker interactionInvoker, IInteractionRequest interactionRequester)
        {
            _translationFactory = translationFactory;
            _interactionInvoker = interactionInvoker;
            _interactionRequester = interactionRequester;
            UpdateTranslation(translationFactory);
            translationFactory.TranslationChanged += (sender, args) => UpdateTranslation(translationFactory);
        }

        public void NotifyWithWindow(ActionResult actionResult)
        {
            Notify(actionResult);
        }

        public void NotifyWithOverlay(ActionResult actionResult)
        {
            Notify(actionResult, false);
        }

        public void NotifyIgnoredWithWindow(ActionResult actionResult)
        {
            Notify(actionResult, justWarn: true);
        }

        private void Notify(ActionResult actionResult, bool withWindow = true, bool justWarn = false)
        {
            var title = justWarn ? _translation.Warning : _translation.Error;
            var icon = justWarn ? MessageIcon.Warning : MessageIcon.Error;
            var errorOccuredText = justWarn ? _translation.ErrorsSkipped : _translation.AnErrorOccured;

            var errorCodeInterpreter = new ErrorCodeInterpreter(_translationFactory);
            var errorText = string.Join(System.Environment.NewLine, actionResult.Select(ar => errorCodeInterpreter.GetErrorText(ar, true)));
            var message = $"{errorOccuredText}{System.Environment.NewLine}{errorText}";

            var messageInteraction = new MessageInteraction(message, title, MessageOptions.OK, icon);

            if (withWindow)
                _interactionInvoker.Invoke(messageInteraction);
            else
            {
                _interactionRequester.Raise(messageInteraction);
            }
        }

        private void UpdateTranslation(ITranslationFactory translationFactory)
        {
            _translation = translationFactory.CreateTranslation<InteractiveWorkflowTranslation>();
        }
    }
}
