using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp
{
    public class SmtpActionViewModel : ActionViewModelBase<SmtpMailAction, SmtpMailTranslation>
    {
        public ICollectionView SmtpAccountsView { get; private set; }
        private ObservableCollection<SmtpAccount> _smtpAccounts;

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }
        public SelectFilesUserControlViewModel AdditionalAttachmentsViewModel { get; private set; }

        private readonly IInteractionRequest _interactionRequest;
        private readonly ISmtpTest _smtpTest;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ICommandLocator _commandLocator;

        public IMacroCommand EditAccountCommand { get; private set; }
        public IMacroCommand AddAccountCommand { get; private set; }
        public AsyncCommand EditDifferingFromCommand { get; private set; }
        public DelegateCommand EditMailTextCommand { get; set; }
        public AsyncCommand TestSmtpCommand { get; set; }

        private EmailSmtpSettings EmailSmtpSettings => CurrentProfile?.EmailSmtpSettings;

        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public SmtpActionViewModel(
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            IInteractionRequest interactionRequest,
            ISmtpTest smtpTest,
            ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings,
            ISelectFilesUserControlViewModelFactory selectFilesUserControlViewModelFactory,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _interactionRequest = interactionRequest;
            _smtpTest = smtpTest;
            _currentSettingsProvider = currentSettingsProvider;
            _commandLocator = commandLocator;
            _gpoSettings = gpoSettings;

            _smtpAccounts = _currentSettingsProvider.CheckSettings.Accounts.SmtpAccounts;

            if (_smtpAccounts != null)
            {
                SmtpAccountsView = new ListCollectionView(_smtpAccounts);
                SmtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(SmtpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            SetTokenViewModel(tokenViewModelFactory);

            AdditionalAttachmentsViewModel = selectFilesUserControlViewModelFactory.Builder()
                .WithTitleGetter(() => Translation.MailAttachmentTitle)
                .WithFileListGetter(profile => profile.EmailSmtpSettings.AdditionalAttachments)
                .WithPropertyChanged(StatusChanged)
                .Build();

            AddAccountCommand = _commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = _commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .AddCommand(new DelegateCommand(o => StatusChanged()))
                .Build();

            EditDifferingFromCommand = new AsyncCommand(EditDifferingFromExecute);

            EditMailTextCommand = new DelegateCommand(EditMailTextExecute);
            TestSmtpCommand = new AsyncCommand(TestSmtpExecute);

            SmtpAccountsView.CurrentChanged += (sender, args) => RaisePropertyChanged(nameof(SenderPreview));
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
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _smtpAccounts.Last();
            SmtpAccountsView.MoveCurrentTo(latestAccount);
            RaisePropertyChanged(nameof(SenderPreview));
        }

        private void RefreshAccountsView()
        {
            SmtpAccountsView.Refresh();
        }

        private Task TestSmtpExecute(object obj)
        {
            return _smtpTest.SendTestMail(CurrentProfile.EmailSmtpSettings, _currentSettingsProvider.CheckSettings.Accounts);
        }

        private void EditMailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailSmtpSettings);

            _interactionRequest.Raise(interaction, EditEmailTextCallback);
        }

        private void EditEmailTextCallback(EditEmailTextInteraction interaction)
        {
            if (!interaction.Success)
                return;

            EmailSmtpSettings.AddSignature = interaction.AddSignature;
            EmailSmtpSettings.Content = interaction.Content;
            EmailSmtpSettings.Subject = interaction.Subject;
            EmailSmtpSettings.Format = interaction.Format;
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var senderPreview = BuildSenderPreview();

                var preview = new StringBuilder(senderPreview);
                preview.AppendLine().Append(Translation.RecipientsToText).Append(" ").Append(CurrentProfile.EmailSmtpSettings.Recipients);

                if (!string.IsNullOrEmpty(CurrentProfile.EmailSmtpSettings.RecipientsCc))
                    preview.AppendLine().Append(Translation.RecipientsCcText).Append(" ").Append(CurrentProfile.EmailSmtpSettings.RecipientsCc);

                if (!string.IsNullOrEmpty(CurrentProfile.EmailSmtpSettings.RecipientsBcc))
                    preview.AppendLine().Append(Translation.RecipientsBccText).Append(" ").Append(CurrentProfile.EmailSmtpSettings.RecipientsBcc);

                return preview.ToString();
            }
        }

        public string SenderPreview => BuildSenderPreview();

        private string BuildSenderPreview()
        {
            var smtpAccount = Accounts.SmtpAccounts.FirstOrDefault(x => x.AccountId == CurrentProfile.EmailSmtpSettings.AccountId);
            if (smtpAccount == null)
                return string.Empty;

            var senderPreview = smtpAccount.Address;
            var withDisplayName = !string.IsNullOrEmpty(CurrentProfile.EmailSmtpSettings.DisplayName);

            if (!string.IsNullOrEmpty(CurrentProfile.EmailSmtpSettings.OnBehalfOf))
            {
                senderPreview = smtpAccount.Address + " " + Translation.OnBehalfOf + " ";
                if (withDisplayName)
                    senderPreview += CurrentProfile.EmailSmtpSettings.DisplayName + " <" + CurrentProfile.EmailSmtpSettings.OnBehalfOf + ">";
                else
                    senderPreview += CurrentProfile.EmailSmtpSettings.OnBehalfOf;
            }
            else
            {
                if (withDisplayName)
                    senderPreview = CurrentProfile.EmailSmtpSettings.DisplayName + " <" + smtpAccount.Address + ">";
            }

            if (!string.IsNullOrEmpty(CurrentProfile.EmailSmtpSettings.ReplyTo))
                senderPreview += Environment.NewLine + Translation.ReplyTo + " " + CurrentProfile.EmailSmtpSettings.ReplyTo;

            return senderPreview;
        }

        private async Task EditDifferingFromExecute(object obj)
        {
            var interaction = new EditEmailDifferingFromInteraction(CurrentProfile.EmailSmtpSettings.Copy());
            await _interactionRequest.RaiseAsync(interaction);
            if (interaction.Success)
                CurrentProfile.EmailSmtpSettings.ReplaceWith(interaction.EmailSmtpSettings);

            RaisePropertyChanged(nameof(SenderPreview));
        }

        public override void MountView()
        {
            RecipientsTokenViewModel.MountView();
            RecipientsCcTokenViewModel.MountView();
            RecipientsBccTokenViewModel.MountView();
            EditAccountCommand.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            RecipientsTokenViewModel.UnmountView();
            RecipientsCcTokenViewModel.UnmountView();
            RecipientsBccTokenViewModel.UnmountView();
            EditAccountCommand.UnmountView();
        }
    }
}
