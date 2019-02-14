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
    public class TimeServerAccountAddCommand : TranslatableCommandBase<TimeServerTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;

        public TimeServerAccountAddCommand(IInteractionRequest interactionRequest,
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
            var newAccount = new TimeServerAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new TimeServerAccountInteraction(newAccount, Translation.AddTimeServerAccount);
            _interactionRequest.Raise(interaction, AddTimeServerAccountCallback);
        }

        private void AddTimeServerAccountCallback(TimeServerAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            var timeServerAccounts = _accountsProvider?.Settings.TimeServerAccounts;
            timeServerAccounts?.Add(interaction.TimeServerAccount);

            var collectionView = CollectionViewSource.GetDefaultView(timeServerAccounts);
            collectionView.MoveCurrentTo(interaction.TimeServerAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
