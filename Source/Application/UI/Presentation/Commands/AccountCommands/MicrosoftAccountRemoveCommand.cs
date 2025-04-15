using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using pdfforge.PDFCreator.Utilities.Messages;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class MicrosoftAccountRemoveCommand : TranslatableCommandBase<MicrosoftTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        private ObservableCollection<ConversionProfile> _profiles => _profilesProvider.Settings;
        private MicrosoftAccount _currentAccount;
        private List<ConversionProfile> _accountUsedInProfiles;

        public MicrosoftAccountRemoveCommand(IInteractionRequest interactionRequest,
            ICurrentSettings<Accounts> accountsProvider,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _accountsProvider = accountsProvider;
            _profilesProvider = profilesProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return _accountsProvider.Settings.MicrosoftAccounts?.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as MicrosoftAccount;
            if (_currentAccount == null)
                return;

            _accountUsedInProfiles = _profiles.Where(p => p.EmailWebSettings.AccountId.Equals(_currentAccount.AccountId) || p.OneDriveSettings.AccountId.Equals(_currentAccount.AccountId)).ToList();

            var title = Translation.RemoveOutlookAccount;
            var message = GetRemoveAccountInteractionMessage(_accountUsedInProfiles);
            var icon = _accountUsedInProfiles.Count > 0 ? MessageIcon.Warning : MessageIcon.Question;
            var interaction = new MessageInteraction(message, title, MessageOptions.YesNo, icon);
            _interactionRequest.Raise(interaction, DeleteAccountCallback);
        }

        private string GetRemoveAccountInteractionMessage(List<ConversionProfile> profiles)
        {
            var sb = new StringBuilder();

            sb.AppendLine(_currentAccount.AccountInfo);
            sb.AppendLine();
            sb.AppendLine(Translation.SureYouWantToDeleteAccount);

            var numProfiles = profiles.Count;
            if (numProfiles <= 0) 
                return sb.ToString();

            sb.AppendLine(Translation.GetAccountIsUsedInFollowingMessage(numProfiles));
            sb.AppendLine();

            foreach (var profile in profiles)
            {
                sb.AppendLine(profile.Name);
            }

            sb.AppendLine();
            sb.AppendLine(Translation.GetActionsGetDisabledMessage(_accountUsedInProfiles.Count));

            return sb.ToString();
        }

        private void DeleteAccountCallback(MessageInteraction interaction)
        {
            if (interaction.Response != MessageResponse.Yes)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            if (_accountsProvider.Settings.MicrosoftAccounts.Contains(_currentAccount))
                _accountsProvider.Settings.MicrosoftAccounts.Remove(_currentAccount);


            foreach (var profile in _accountUsedInProfiles)
            {
                if (profile.OneDriveSettings.AccountId == _currentAccount.AccountId)
                {
                    profile.OneDriveSettings.AccountId = "";
                    profile.OneDriveSettings.Enabled = false;
                    profile.ActionOrder.Remove(nameof(OneDriveSettings));
                }

                if (profile.EmailWebSettings.AccountId == _currentAccount.AccountId)
                {
                    profile.EmailWebSettings.AccountId = "";
                    profile.EmailWebSettings.Enabled = false;
                    profile.ActionOrder.Remove(nameof(EmailWebSettings));
                }
            }

            // todo do we want to log the user out? Would log them out of everything not only our application

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
