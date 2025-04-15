using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using System;
using pdfforge.PDFCreator.Utilities.Messages;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    public class EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand : EvaluateSettingsAndNotifyUserCommandBase
    {
        private readonly ITabSwitchSettingsCheck _tabSwitchSettingsCheck;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;

        public EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand(
            IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater,
            ITabSwitchSettingsCheck tabSwitchSettingsCheck,
        ICurrentSettingsProvider currentSettingsProvider)
            : base(
                interactionRequest,
                translationUpdater)
        {
            _tabSwitchSettingsCheck = tabSwitchSettingsCheck;
            _currentSettingsProvider = currentSettingsProvider;
        }

        protected override SettingsCheckResult EvaluateRelevantSettings()
        {
            return _tabSwitchSettingsCheck.CheckAffectedSettings();
        }

        protected override MessageInteraction DetermineInteraction(SettingsCheckResult result)
        {
            bool withErrors = !result.Result;
            var withChanges = result.SettingsHaveChanged;

            if (!withChanges && !withErrors)
                return null;

            if (withChanges && !withErrors)
            {
                var title = Translation.Settings;
                var text = Translation.UnsavedChanges
                           + Environment.NewLine
                           + Translation.HowToProceed;
                var buttons = MessageOptions.SaveDiscardBack;
                return new MessageInteraction(text, title, buttons, MessageIcon.Question);
            }

            if (!withChanges && withErrors)
                return null;

            if (withChanges && withErrors)
            {
                var title = Translation.Settings;
                var text = Translation.InvalidSettingsWithUnsavedChanges;
                var buttons = MessageOptions.SaveDiscardBack;
                var userQuestion = Translation.HowToProceed;
                return new MessageInteraction(text, title, buttons, MessageIcon.Warning, result.Result, userQuestion);
            }

            return null;
        }

        protected override void ResolveInteractionResult(MessageInteraction interactionResult)
        {
            switch (interactionResult.Response)
            {
                case MessageResponse.Save:
                    RaiseIsDone(ResponseStatus.Success);
                    return;

                case MessageResponse.Discard:
                    _currentSettingsProvider.Reset(false);
                    RaiseIsDone(ResponseStatus.Skip);
                    return;

                case MessageResponse.Back:
                case MessageResponse.Cancel:
                default:
                    RaiseIsDone(ResponseStatus.Cancel);
                    return;
            }
        }
    }
}
