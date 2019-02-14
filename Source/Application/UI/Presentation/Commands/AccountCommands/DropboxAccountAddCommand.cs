using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class DropboxAccountAddCommand : TranslatableCommandBase<DropboxTranslation>, IWaitableCommand
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;

        public DropboxAccountAddCommand(
            IInteractionInvoker interactionInvoker,
            IInteractionRequest interactionRequest,
            ICurrentSettings<Conversion.Settings.Accounts> accountsProvider,
            ITranslationUpdater translationUpdater
            )
            : base(translationUpdater)
        {
            _interactionInvoker = interactionInvoker;
            _interactionRequest = interactionRequest;
            _accountsProvider = accountsProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var interaction = new DropboxAccountInteraction();
            interaction.Result = DropboxAccountInteractionResult.UserCanceled; //required as default for [X]-Button
            _interactionInvoker.Invoke(interaction);

            switch (interaction.Result)
            {
                case DropboxAccountInteractionResult.Success:
                    break;

                case DropboxAccountInteractionResult.AccesTokenParsingError:
                    var parseErrorMessageInteraction = new MessageInteraction(Translation.DropboxAccountSeverResponseErrorMessage, Translation.AddDropboxAccount, MessageOptions.OK, MessageIcon.Warning);
                    _interactionRequest.Raise(parseErrorMessageInteraction, IsDoneWithErrorCallback);
                    return;

                case DropboxAccountInteractionResult.Error:
                    var errorMessageInteraction = new MessageInteraction(Translation.DropboxAccountCreationErrorMessage, Translation.AddDropboxAccount, MessageOptions.OK, MessageIcon.Warning);
                    _interactionRequest.Raise(errorMessageInteraction, IsDoneWithErrorCallback);
                    return;

                case DropboxAccountInteractionResult.UserCanceled:
                default:
                    IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                    return;
            }

            var newAccount = interaction.DropboxAccount;

            var accountWithSameID = _accountsProvider.Settings.DropboxAccounts.FirstOrDefault(a => a.AccountId == newAccount.AccountId);
            if (accountWithSameID != null)
                _accountsProvider.Settings.DropboxAccounts.Remove(accountWithSameID);

            _accountsProvider.Settings.DropboxAccounts.Add(newAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private void IsDoneWithErrorCallback(MessageInteraction interaction)
        {
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Error));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
