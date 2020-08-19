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
    public class HttpAccountEditCommand : TranslatableCommandBase<HttpTranslation>, IWaitableCommand, IMountable
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private HttpAccount _currentAccount;
        private ObservableCollection<HttpAccount> _pointAtAccounts;

        public HttpAccountEditCommand(IInteractionRequest interactionRequest,
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
            if (_pointAtAccounts != _accountsProvider.Settings.HttpAccounts)
            {
                _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
                _pointAtAccounts = _accountsProvider.Settings.HttpAccounts;
                _pointAtAccounts.CollectionChanged += OnCollectionChanged;
            }
        }

        public override bool CanExecute(object parameter)
        {
            return _accountsProvider?.Settings.HttpAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as HttpAccount;
            if (_currentAccount == null)
                return;

            var httpAccounts = _accountsProvider?.Settings.HttpAccounts;
            if (!httpAccounts.Contains(_currentAccount))
                return;

            var interaction = new HttpAccountInteraction(_currentAccount.Copy(), Translation.EditHttpAccount);
            _interactionRequest.Raise(interaction, UpdateHttpAccountsCallback);
        }

        private void UpdateHttpAccountsCallback(HttpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            interaction.HttpAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

        public void MountView()
        {
            _accountsProvider.SettingsChanged += OnSettingsChanged;
            _pointAtAccounts = _accountsProvider.Settings.HttpAccounts;
            _pointAtAccounts.CollectionChanged += OnCollectionChanged;
        }

        public void UnmountView()
        {
            _accountsProvider.SettingsChanged -= OnSettingsChanged;
            _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
        }
    }
}
