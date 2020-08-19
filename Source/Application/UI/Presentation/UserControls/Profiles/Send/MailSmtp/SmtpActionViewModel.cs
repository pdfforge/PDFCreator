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
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp
{
    public class SmtpActionViewModel : MailBaseControlViewModel<SmtpTranslation>
    {
        public ICollectionView SmtpAccountsView { get; private set; }

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }

        private ObservableCollection<SmtpAccount> _smtpAccounts;

        private readonly IInteractionRequest _interactionRequest;
        private readonly ISmtpTest _smtpTest;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountsProvider;
        private readonly ICommandLocator _commandLocator;

        public IMacroCommand EditAccountCommand { get; private set; }
        public IMacroCommand AddAccountCommand { get; private set; }
        public DelegateCommand EditMailTextCommand { get; set; }
        public AsyncCommand TestSmtpCommand { get; set; }

        private EmailSmtpSettings EmailSmtpSettings => CurrentProfile?.EmailSmtpSettings;
        protected override IMailActionSettings MailActionSettings => EmailSmtpSettings;

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
            IGpoSettings gpoSettings,
            IOpenFileInteractionHelper openFileInteractionHelper)
            : base(updater, currentSettingsProvider, dispatcher, openFileInteractionHelper)
        {
            _interactionRequest = interactionRequest;
            _smtpTest = smtpTest;
            _accountsProvider = accountsProvider;
            _commandLocator = commandLocator;
            _gpoSettings = gpoSettings;

            _smtpAccounts = _accountsProvider?.Settings.SmtpAccounts;

            if (_smtpAccounts != null)
            {
                SmtpAccountsView = new ListCollectionView(_smtpAccounts);
                SmtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(SmtpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            SetTokenViewModel(tokenViewModelFactory);

            AddAccountCommand = _commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = _commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();

            EditMailTextCommand = new DelegateCommand(EditMailTextExecute);
            TestSmtpCommand = new AsyncCommand(TestSmtpExecute);
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

            AdditionalAttachmentsTokenViewModel = builder
                .WithSelector(p => AdditionalAttachmentsForTextBox)
                .WithButtonCommand(SelectAttachmentFileAction)
                .WithSecondaryButtonCommand(AddFilePathToAdditionalAttachments)
                .Build();
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

        private Task TestSmtpExecute(object obj)
        {
            return _smtpTest.SendTestMail(CurrentProfile, _accountsProvider.Settings);
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

        public override void MountView()
        {
            RecipientsTokenViewModel.MountView();
            RecipientsCcTokenViewModel.MountView();
            RecipientsBccTokenViewModel.MountView();
            AdditionalAttachmentsTokenViewModel.MountView();
            EditAccountCommand.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            RecipientsTokenViewModel.UnmountView();
            RecipientsCcTokenViewModel.UnmountView();
            RecipientsBccTokenViewModel.UnmountView();
            AdditionalAttachmentsTokenViewModel.UnmountView();
            EditAccountCommand.UnmountView();
        }
    }
}
