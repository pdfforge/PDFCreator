using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using System;

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
                           + Translation.WantToSave
                           + Environment.NewLine
                           + Translation.ChooseNoToRevert;
                var buttons = MessageOptions.YesNoCancel;
                return new MessageInteraction(text, title, buttons, MessageIcon.Question);
            }

            if (!withChanges && withErrors)
                return null;

            if (withChanges && withErrors)
            {
                var title = Translation.Settings;
                var text = Translation.InvalidSettingsWithUnsavedChanges;
                var buttons = MessageOptions.YesNoCancel;
                var userQuestion = Translation.WantToSaveAnyway
                                   + Environment.NewLine
                                   + Translation.ChooseNoToRevert;
                return new MessageInteraction(text, title, buttons, MessageIcon.Warning, result.Result, userQuestion);
            }

            return null;
        }

        protected override void ResolveInteractionResult(MessageInteraction interactionResult)
        {
            switch (interactionResult.Response)
            {
                case MessageResponse.Yes:
                    RaiseIsDone(ResponseStatus.Success);
                    return;

                case MessageResponse.No:
                    _currentSettingsProvider.Reset();
                    RaiseIsDone(ResponseStatus.Skip);
                    return;

                case MessageResponse.Cancel:
                default:
                    RaiseIsDone(ResponseStatus.Cancel);
                    return;
            }
        }
    }
}
