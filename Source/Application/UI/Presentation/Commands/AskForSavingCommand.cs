using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class AskForSavingCommand : TranslatableCommandBase<EvaluateSettingsAndNotifyUserTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ITabSwitchSettingsCheck _tabSwitchSettingsCheck;

        public AskForSavingCommand(ITranslationUpdater translationUpdater, IInteractionRequest interactionRequest, ITabSwitchSettingsCheck tabSwitchSettingsCheck) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _tabSwitchSettingsCheck = tabSwitchSettingsCheck;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
            var settingsCheckResult = _tabSwitchSettingsCheck.CheckAllSettings();
            if (settingsCheckResult.SettingsHaveChanged)
            {
                var title = Translation.Settings;
                var text = Translation.UnsavedChanges
                           + Environment.NewLine
                           + Translation.WantToSave;

                var messageInteraction = new MessageInteraction(text, title, MessageOptions.YesCancel, MessageIcon.Question);

                await _interactionRequest.RaiseAsync(messageInteraction);
                if (messageInteraction.Response != MessageResponse.Yes)
                {
                    IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                }
            }
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
