using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using pdfforge.PDFCreator.Core.Services;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class TimeServerAccountEditCommand : TranslatableCommandBase<TimeServerTranslation>, IWaitableCommand, IMountable
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private ObservableCollection<TimeServerAccount> TimeServerAccounts => _accountsProvider?.Settings.TimeServerAccounts;
        private TimeServerAccount _currentAccount;
        private ObservableCollection<TimeServerAccount> _pointAtAccounts;

        public TimeServerAccountEditCommand
        (
            IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater
        )
        : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _accountsProvider = accountsProvider;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (_pointAtAccounts != _accountsProvider.Settings.TimeServerAccounts)
            {
                _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
                _pointAtAccounts = _accountsProvider.Settings.TimeServerAccounts;
                _pointAtAccounts.CollectionChanged += OnCollectionChanged;
            }
        }

        public override bool CanExecute(object parameter)
        {
            return TimeServerAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as TimeServerAccount;
            if (_currentAccount == null)
                return;
            if (!TimeServerAccounts.Contains(_currentAccount))
                return;

            var interaction = new TimeServerAccountInteraction(_currentAccount.Copy(), Translation.EditTimeServerAccount);
            _interactionRequest.Raise(interaction, UpdateTimeServerAccountsCallback);
        }

        private void UpdateTimeServerAccountsCallback(TimeServerAccountInteraction interaction)
        {
            if (interaction.Success)
            {
                interaction.TimeServerAccount.CopyTo(_currentAccount);
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            }
            else
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
            }
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

        public void MountView()
        {
            _accountsProvider.SettingsChanged += OnSettingsChanged;
            _pointAtAccounts = _accountsProvider.Settings.TimeServerAccounts;
            _pointAtAccounts.CollectionChanged += OnCollectionChanged;
        }

        public void UnmountView()
        {
            _accountsProvider.SettingsChanged -= OnSettingsChanged;
            _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
        }
    }
}
