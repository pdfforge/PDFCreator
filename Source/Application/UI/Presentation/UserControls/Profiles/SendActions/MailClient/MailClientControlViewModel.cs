using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailClient
{
    public class MailClientControlViewModel : ActionViewModelBase<EMailClientAction, MailTranslation>
    {
        private readonly IClientTestMailAssistant _clientTestMailAssistant;
        private readonly IInteractionRequest _interactionRequest;

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }
        public SelectFilesUserControlViewModel AdditionalAttachmentsViewModel { get; private set; }

        public AsyncCommand EmailClientTestCommand { get; set; }
        public DelegateCommand EditEmailTextCommand { get; set; }
        private EmailClientSettings EmailClientSettings => CurrentProfile?.EmailClientSettings;

        public MailClientControlViewModel(
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IInteractionRequest interactionRequest,
            IClientTestMailAssistant clientTestMailAssistant,
            ITranslationUpdater translationUpdater,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            ISelectFilesUserControlViewModelFactory selectFilesUserControlViewModelFactory,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _interactionRequest = interactionRequest;
            _clientTestMailAssistant = clientTestMailAssistant;

            CreateTokenViewModels(tokenViewModelFactory);

            EmailClientTestCommand = new AsyncCommand(EmailClientTestExecute);
            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);

            AdditionalAttachmentsViewModel = selectFilesUserControlViewModelFactory.Builder()
                .WithTitleGetter(() => Translation.MailAttachmentTitle)
                .WithFileListGetter(profile => profile.EmailClientSettings.AdditionalAttachments)
                .WithPropertyChanged(StatusChanged)
                .Build();
        }

        private void CreateTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForEmailRecipients());

            RecipientsTokenViewModel = builder
                .WithSelector(p => p.EmailClientSettings.Recipients)
                .Build();

            RecipientsCcTokenViewModel = builder
                .WithSelector(p => p.EmailClientSettings.RecipientsCc)
                .Build();

            RecipientsBccTokenViewModel = builder
                .WithSelector(p => p.EmailClientSettings.RecipientsBcc)
                .Build();
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var preview = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(CurrentProfile.EmailClientSettings.Recipients))
                    preview.Append(Translation.RecipientsToText).Append(" ").Append(CurrentProfile.EmailClientSettings.Recipients);
                else
                    preview.Append(Translation.BlankToField);

                if (!string.IsNullOrEmpty(CurrentProfile.EmailClientSettings.RecipientsCc))
                    preview.AppendLine().Append(Translation.RecipientsCcText).Append(" ").Append(CurrentProfile.EmailClientSettings.RecipientsCc);

                if (!string.IsNullOrEmpty(CurrentProfile.EmailClientSettings.RecipientsBcc))
                    preview.AppendLine().Append(Translation.RecipientsBccText).Append(" ").Append(CurrentProfile.EmailClientSettings.RecipientsBcc);

                return preview.ToString();
            }
        }

        public override void MountView()
        {
            RecipientsTokenViewModel.MountView();
            RecipientsCcTokenViewModel.MountView();
            RecipientsBccTokenViewModel.MountView();
            AdditionalAttachmentsViewModel.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            RecipientsTokenViewModel.UnmountView();
            RecipientsCcTokenViewModel.UnmountView();
            RecipientsBccTokenViewModel.UnmountView();
            AdditionalAttachmentsViewModel.UnmountView();
        }

        private Task EmailClientTestExecute(object obj)
        {
            return _clientTestMailAssistant.SendTestEmail(EmailClientSettings);
        }

        private void EditEmailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailClientSettings);

            _interactionRequest.Raise(interaction, EditEmailTextCallback);
        }

        private void EditEmailTextCallback(EditEmailTextInteraction interaction)
        {
            if (!interaction.Success)
                return;

            EmailClientSettings.AddSignature = interaction.AddSignature;
            EmailClientSettings.Content = interaction.Content;
            EmailClientSettings.Subject = interaction.Subject;
            EmailClientSettings.Format = interaction.Format;
        }
    }
}
