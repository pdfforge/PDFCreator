using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class MicrosoftAccountEditCommand : TranslatableCommandBase<MicrosoftTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private MicrosoftAccount _currentAccount;
        

        public MicrosoftAccountEditCommand(
            IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var title = Translation.EditMicrosoftAccount;
            _currentAccount = parameter as MicrosoftAccount;
            if (_currentAccount == null)
            {
                _currentAccount = new MicrosoftAccount();
                title = Translation.AddMicrosoftAccount;
            }
            
            var interaction = new MicrosoftAccountInteraction(_currentAccount, title);
            _interactionRequest.Raise(interaction, UpdateMicrosoftAccountsCallback);
        }


        private void UpdateMicrosoftAccountsCallback(MicrosoftAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }
            
            interaction.MicrosoftAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
