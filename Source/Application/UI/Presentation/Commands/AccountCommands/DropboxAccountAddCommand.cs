using NLog;
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
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private readonly IDropboxUserInfoManager _dropboxUserInfoManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DropboxAccountAddCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ITranslationUpdater translationUpdater,
            IDropboxUserInfoManager dropboxUserInfoManager
        )
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _accountsProvider = accountsProvider;
            _dropboxUserInfoManager = dropboxUserInfoManager;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
            var newAccount = new DropboxAccount();
            try
            {
                var userInfo = await _dropboxUserInfoManager.GetDropboxUserInfo();
                if (userInfo.AccessToken != null)
                {
                    newAccount.AccountId = userInfo.AccountId;
                    newAccount.AccessToken = userInfo.AccessToken;
                    newAccount.AccountInfo = userInfo.AccountInfo;
                    newAccount.RefreshToken = userInfo.RefreshToken;
                }
            }
            catch (DropboxAccessDeniedException)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured trying to add Dropbox account.");
                var errorMessageInteraction = new MessageInteraction(Translation.DropboxAccountCreationErrorMessage, Translation.AddDropboxAccount, MessageOptions.OK, MessageIcon.Warning);
                _interactionRequest.Raise(errorMessageInteraction, IsDoneWithErrorCallback);
                return;
            }

            var accountWithSameId = _accountsProvider.Settings.DropboxAccounts.FirstOrDefault(a => a.AccountId == newAccount.AccountId);
            if (accountWithSameId != null)
                _accountsProvider.Settings.DropboxAccounts.Remove(accountWithSameId);

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
