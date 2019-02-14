using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp
{
    public class SmtpActionViewModel : ProfileUserControlViewModel<SmtpTranslation>
    {
        public ICollectionView SmtpAccountsView { get; }

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }

        private readonly ObservableCollection<SmtpAccount> _smtpAccounts;

        private readonly IInteractionRequest _interactionRequest;
        private readonly ISmtpTest _smtpTest;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountsProvider;

        public IMacroCommand EditAccountCommand { get; }
        public IMacroCommand AddAccountCommand { get; }
        public DelegateCommand EditMailTextCommand { get; set; }
        public DelegateCommand TestSmtpCommand { get; set; }

        private EmailSmtpSettings EmailSmtpSettings => CurrentProfile?.EmailSmtpSettings;

        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public SmtpActionViewModel(IInteractionRequest interactionRequest,
            ISmtpTest smtpTest,
            ITranslationUpdater updater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICurrentSettings<Conversion.Settings.Accounts> accountsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings) : base(updater, currentSettingsProvider, dispatcher)
        {
            _interactionRequest = interactionRequest;
            _smtpTest = smtpTest;
            _accountsProvider = accountsProvider;
            _gpoSettings = gpoSettings;

            SetTokenViewModel(tokenViewModelFactory);

            _smtpAccounts = _accountsProvider?.Settings.SmtpAccounts;

            if (_smtpAccounts != null)
            {
                SmtpAccountsView = new ListCollectionView(_smtpAccounts);
                SmtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(SmtpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            AddAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();

            EditMailTextCommand = new DelegateCommand(EditMailTextExecute);
            TestSmtpCommand = new DelegateCommand(TextSmtpExecute);
        }

        private void SetTokenViewModel(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory.BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForEmailRecipients());

            RecipientsTokenViewModel = builder
                .WithSelector(p => p.EmailSmtpSettings.Recipients)
                .Build();

            RecipientsCcTokenViewModel = builder
                .WithSelector(p => p.EmailSmtpSettings.RecipientsCc)
                .Build();

            RecipientsBccTokenViewModel = builder
                .WithSelector(p => p.EmailSmtpSettings.RecipientsBcc)
                .Build();

            RaisePropertyChanged(nameof(RecipientsTokenViewModel));
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _smtpAccounts.Last();
            SmtpAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            SmtpAccountsView.Refresh();
        }

        private void TextSmtpExecute(object obj)
        {
            _smtpTest.SendTestMail(CurrentProfile, _accountsProvider.Settings);
        }

        private void EditMailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailSmtpSettings.Subject, EmailSmtpSettings.Content, EmailSmtpSettings.AddSignature, EmailSmtpSettings.Html);

            _interactionRequest.Raise(interaction, EditEmailTextCallback);

            if (!interaction.Success)
                return;

            EmailSmtpSettings.AddSignature = interaction.AddSignature;
            EmailSmtpSettings.Content = interaction.Content;
            EmailSmtpSettings.Subject = interaction.Subject;
            EmailSmtpSettings.Html = interaction.Html;
        }

        private void EditEmailTextCallback(EditEmailTextInteraction interaction)
        {
            if (!interaction.Success)
                return;

            EmailSmtpSettings.AddSignature = interaction.AddSignature;
            EmailSmtpSettings.Content = interaction.Content;
            EmailSmtpSettings.Subject = interaction.Subject;
            EmailSmtpSettings.Html = interaction.Html;
        }
    }
}
