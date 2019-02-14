using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class HttpAccountAddCommand : TranslatableCommandBase<HttpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;

        public HttpAccountAddCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater)
            : base(translationUpdater)
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
            var newAccount = new HttpAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new HttpAccountInteraction(newAccount, Translation.AddHttpAccount);
            _interactionRequest.Raise(interaction, AddHttpAccountCallback);
        }

        private void AddHttpAccountCallback(HttpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            var httpAccounts = _accountsProvider.Settings.HttpAccounts;
            httpAccounts.Add(interaction.HttpAccount);

            var collectionView = CollectionViewSource.GetDefaultView(httpAccounts);
            collectionView.MoveCurrentTo(interaction.HttpAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
