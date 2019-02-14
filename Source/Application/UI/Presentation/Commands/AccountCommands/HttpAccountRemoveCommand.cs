using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class HttpAccountRemoveCommand : TranslatableCommandBase<HttpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private ObservableCollection<HttpAccount> HttpAccounts => _accountsProvider.Settings.HttpAccounts;
        private ObservableCollection<ConversionProfile> Profiles => _profilesProvider.Settings;
        private HttpAccount _currentAccount;
        private List<ConversionProfile> _usedInProfilesList;

        public HttpAccountRemoveCommand(IInteractionRequest interactionRequest,
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
            return HttpAccounts?.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as HttpAccount;
            if (_currentAccount == null)
                return;

            _usedInProfilesList = Profiles.Where(p => p.HttpSettings.AccountId.Equals(_currentAccount.AccountId)).ToList();

            var title = Translation.RemoveHttpAccount;

            var messageSb = new StringBuilder();
            messageSb.AppendLine(_currentAccount.AccountInfo);
            messageSb.AppendLine(Translation.SureYouWantToDeleteAccount);

            if (_usedInProfilesList.Count > 0)
            {
                messageSb.AppendLine();
                messageSb.AppendLine(Translation.GetAccountIsUsedInFollowingMessage(_usedInProfilesList.Count));
                messageSb.AppendLine();
                foreach (var profile in _usedInProfilesList)
                {
                    messageSb.AppendLine(profile.Name);
                }
                messageSb.AppendLine();
                messageSb.AppendLine(Translation.GetHttppGetsDisabledMessage(_usedInProfilesList.Count));
            }
            var message = messageSb.ToString();
            var icon = _usedInProfilesList.Count > 0 ? MessageIcon.Warning : MessageIcon.Question;
            var interaction = new MessageInteraction(message, title, MessageOptions.YesNo, icon);
            _interactionRequest.Raise(interaction, DeleteAccountCallback);
        }

        private void DeleteAccountCallback(MessageInteraction interaction)
        {
            if (interaction.Response != MessageResponse.Yes)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }
            if (HttpAccounts.Contains(_currentAccount))
                HttpAccounts.Remove(_currentAccount);

            foreach (var profile in _usedInProfilesList)
            {
                profile.HttpSettings.AccountId = "";
                profile.HttpSettings.Enabled = false;
            }

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
