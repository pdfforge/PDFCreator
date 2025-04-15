using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using System;
using pdfforge.PDFCreator.Utilities.Messages;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IInteractiveProfileChecker
    {
        bool CheckWithErrorResultInOverlay(Job job);

        bool CheckWithErrorResultInWindow(Job job);
    }

    public class InteractiveProfileChecker : IInteractiveProfileChecker
    {
        private readonly IProfileChecker _profileChecker;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ITranslationFactory _translationFactory;

        private InteractiveProfileCheckerTranslation _translation;

        public InteractiveProfileChecker(IProfileChecker profileChecker, IInteractionRequest interactionRequest,
            IInteractionInvoker interactionInvoker, ITranslationFactory translationFactory)
        {
            _profileChecker = profileChecker;
            _interactionRequest = interactionRequest;
            _interactionInvoker = interactionInvoker;
            _translationFactory = translationFactory;
        }

        public bool CheckWithErrorResultInOverlay(Job job)
        {
            return CheckIfProfileIsValidElseNotifyUser(job, LaunchOverlay);
        }

        public bool CheckWithErrorResultInWindow(Job job)
        {
            return CheckIfProfileIsValidElseNotifyUser(job, LaunchWindow);
        }

        private bool CheckIfProfileIsValidElseNotifyUser(Job job, Action<MessageInteraction> notifyUser)
        {
            var profileCheckResult = _profileChecker.CheckJob(job);

            if (!profileCheckResult)
            {
                var interaction = BuildInteraction(job, profileCheckResult);
                notifyUser(interaction);
                return false;
            }
            return true;
        }

        private MessageInteraction BuildInteraction(Job job, ActionResult profileCheckResult)
        {
            _translation = _translationFactory.UpdateOrCreateTranslation(_translation);

            var title = _translation.Error;
            var text = _translation.InvalidSettings;
            var interaction = new MessageInteraction(text, title, MessageOptions.Ok, MessageIcon.Exclamation, job.Profile.Name, profileCheckResult);
            interaction.ShowErrorRegions = false;
            return interaction;
        }

        private void LaunchOverlay(MessageInteraction interaction)
        {
            _interactionRequest.Raise(interaction);
        }

        private void LaunchWindow(MessageInteraction interaction)
        {
            _interactionInvoker.Invoke(interaction);
        }
    }
}
