using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Linq;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft;
using Logger = NLog.Logger;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class MicrosoftAccountRequestPermissionCommand : TranslatableCommandBase<DropboxTranslation>, IWaitableCommand
    {
        private readonly IGraphManager _graphManager;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MicrosoftAccountRequestPermissionCommand(
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
            return true;
        }

        public override async void Execute(object parameter)
        {
            if (parameter is not MicrosoftAccountPermissionsPayload payload)
                return;

            try
            {
                if (payload.Permissions.IsOnlyOfflinePermission())
                {
                    var existingAccount = _accountsProvider.Settings.MicrosoftAccounts.FirstOrDefault(account => account.AccountId == payload.Account.AccountId);

                    if (existingAccount != null)
                    {
                        existingAccount.PermissionScopes = payload.Permissions.ToPermissionScope();
                    }
                    IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
                    return;
                }
                await _graphManager.AcquireAccessToken(payload.Account, payload.Permissions);
                var clientWrapper = _graphManager.GetClient(payload.Account);
                if (!string.IsNullOrEmpty(clientWrapper.Account.AccountId) && !string.IsNullOrEmpty(clientWrapper.Account.AccountInfo))
                {
                    var existingAccount = _accountsProvider.Settings.MicrosoftAccounts.FirstOrDefault(account => account.AccountId == clientWrapper.Account.AccountId);

                    if (existingAccount != null)
                        clientWrapper.Account.CopyTo(existingAccount);
                    else
                        _accountsProvider.Settings.MicrosoftAccounts.Add(clientWrapper.Account);
                }
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
