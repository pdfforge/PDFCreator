using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using Logger = NLog.Logger;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class MicrosoftAccountAddCommand : TranslatableCommandBase<DropboxTranslation>, IWaitableCommand
    {
        private readonly IGraphManager _graphManager;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MicrosoftAccountAddCommand(
            IGraphManager graphManager,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater
        )
            : base(translationUpdater)
        {
            _graphManager = graphManager;
            _accountsProvider = accountsProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return _accountsProvider.Settings.MicrosoftAccounts.Count == 0;
        }

        public override async void Execute(object parameter)
        {
            var newAccount = new MicrosoftAccount();
            try
            {
                await _graphManager.AddMicrosoftAccount(newAccount);
                _accountsProvider.Settings.MicrosoftAccounts.Clear();

                if (!string.IsNullOrEmpty(newAccount.AccountId) && !string.IsNullOrEmpty(newAccount.AccountInfo))
                    _accountsProvider.Settings.MicrosoftAccounts.Add(newAccount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured trying to add a Graph Account.");
                return;
            }

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private void IsDoneWithErrorCallback(MessageInteraction interaction)
        {
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Error));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
