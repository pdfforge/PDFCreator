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
    public class SmtpAccountEditCommand : TranslatableCommandBase<SmtpTranslation>, IWaitableCommand, IMountable
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private ObservableCollection<SmtpAccount> SmtpAccounts => _accountsProvider.Settings.SmtpAccounts;
        private SmtpAccount _currentAccount;
        private ObservableCollection<SmtpAccount> _pointAtAccounts;

        public SmtpAccountEditCommand(IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater) : base(translationUpdater)
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
            if (_pointAtAccounts != _accountsProvider.Settings.SmtpAccounts)
            {
                _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
                _pointAtAccounts = _accountsProvider.Settings.SmtpAccounts;
                _pointAtAccounts.CollectionChanged += OnCollectionChanged;
            }
        }

        public override bool CanExecute(object parameter)
        {
            return SmtpAccounts?.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as SmtpAccount;
            if (_currentAccount == null)
                return;
            if (!SmtpAccounts.Contains(_currentAccount))
                return;

            var interaction = new SmtpAccountInteraction(_currentAccount.Copy(), Translation.EditSmtpAccount);
            _interactionRequest.Raise(interaction, UpdateSmtpAccountsCallback);
        }

        private void UpdateSmtpAccountsCallback(SmtpAccountInteraction interactionBase)
        {
            if (!interactionBase.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            interactionBase.SmtpAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

        public void MountView()
        {
            _accountsProvider.SettingsChanged += OnSettingsChanged;
            _pointAtAccounts = _accountsProvider.Settings.SmtpAccounts;
            _pointAtAccounts.CollectionChanged += OnCollectionChanged;
        }

        public void UnmountView()
        {
            _accountsProvider.SettingsChanged -= OnSettingsChanged;
            _pointAtAccounts.CollectionChanged -= OnCollectionChanged;
        }
    }
}
