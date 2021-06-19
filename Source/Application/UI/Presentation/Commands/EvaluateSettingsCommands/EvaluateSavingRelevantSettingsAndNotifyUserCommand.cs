using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    public class EvaluateSavingRelevantSettingsAndNotifyUserCommand : EvaluateSettingsAndNotifyUserCommandBase
    {
        private readonly ITabSwitchSettingsCheck _tabSwitchSettingsCheck;

        public EvaluateSavingRelevantSettingsAndNotifyUserCommand(
            IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater,
            ITabSwitchSettingsCheck tabSwitchSettingsCheck)
            : base(interactionRequest, translationUpdater)
        {
            _tabSwitchSettingsCheck = tabSwitchSettingsCheck;
        }

        protected override SettingsCheckResult EvaluateRelevantSettings()
        {
            return _tabSwitchSettingsCheck.CheckAffectedSettings();
        }

        protected override MessageInteraction DetermineInteraction(SettingsCheckResult result)
        {
            var withErrors = !result.Result;

            if (!withErrors)
                return null;

            var withChanges = result.SettingsHaveChanged;

            var title = Translation.Settings;
            var buttons = MessageOptions.YesNo;

            var userQuestion = withChanges ? Translation.WantToSaveAnyway : Translation.WantToProceedAnyway;
            var text = withChanges ? Translation.InvalidSettingsWithUnsavedChanges : Translation.InvalidSettings;

            return new MessageInteraction(text, title, buttons, MessageIcon.Warning, result.Result, userQuestion);
        }

        protected override void ResolveInteractionResult(MessageInteraction interactionResult)
        {
            switch (interactionResult.Response)
            {
                case MessageResponse.Yes:
                    RaiseIsDone(ResponseStatus.Success);
                    return;

                case MessageResponse.No:
                case MessageResponse.Cancel:
                default:
                    RaiseIsDone(ResponseStatus.Cancel);
                    return;
            }
        }
    }
}
