using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class SmtpAccountAddCommand : TranslatableCommandBase<AccountsTranslation>, IWaitableCommand
    {
        private ObservableCollection<SmtpAccount> SmtpAccounts => _accountsProvider?.Settings.SmtpAccounts;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;

        public SmtpAccountAddCommand(IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _accountsProvider = accountsProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var newAccount = new SmtpAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new SmtpAccountInteraction(newAccount, Translation.AddSmtpAccount);
            _interactionRequest.Raise(interaction, AddSmtpAccountCallback);
        }

        private void AddSmtpAccountCallback(SmtpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }
            SmtpAccounts.Add(interaction.SmtpAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
