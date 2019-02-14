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

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class FtpAccountEditCommand : TranslatableCommandBase<FtpActionTranslation>, IWaitableCommand, IMountable
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private FtpAccount _currentAccount;
        private ObservableCollection<FtpAccount> _pointAtAccounts;

        public FtpAccountEditCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater)
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
            if (_pointAtAccounts != _accountsProvider.Settings.FtpAccounts)
            {
                _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
                _pointAtAccounts = _accountsProvider.Settings.FtpAccounts;
                _pointAtAccounts.CollectionChanged += OnCollectionChanged;
            }
        }

        public override bool CanExecute(object parameter)
        {
            return _accountsProvider.Settings.FtpAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as FtpAccount;
            if (_currentAccount == null)
                return;
            if (!_accountsProvider.Settings.FtpAccounts.Contains(_currentAccount))
                return;

            var interaction = new FtpAccountInteraction(_currentAccount.Copy(), Translation.EditFtpAccount);
            _interactionRequest.Raise(interaction, UpdateFtpAccountsCallback);
        }

        private void UpdateFtpAccountsCallback(FtpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            interaction.FtpAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

        public void MountView()
        {
            _accountsProvider.SettingsChanged += OnSettingsChanged;
            _pointAtAccounts = _accountsProvider.Settings.FtpAccounts;
            _pointAtAccounts.CollectionChanged += OnCollectionChanged;
        }

        public void UnmountView()
        {
            _accountsProvider.SettingsChanged -= OnSettingsChanged;
        }
    }
}
