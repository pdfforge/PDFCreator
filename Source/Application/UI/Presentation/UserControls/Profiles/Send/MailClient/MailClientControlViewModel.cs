using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient
{
    public class MailClientControlViewModel : ProfileUserControlViewModel<MailTranslation>
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

        public MailClientControlViewModel(IInteractionRequest interactionRequest,
            IClientTestMailAssistant clientTestMailAssistant,
            ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            ISelectFilesUserControlViewModelFactory selectFilesUserControlViewModelFactory)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _interactionRequest = interactionRequest;
            _clientTestMailAssistant = clientTestMailAssistant;

            CreateTokenViewModels(tokenViewModelFactory);

            EmailClientTestCommand = new AsyncCommand(EmailClientTestExecute);
            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);

            AdditionalAttachmentsViewModel = selectFilesUserControlViewModelFactory.Builder()
                .WithTitleGetter(() => Translation.MailAttachmentTitle)
                .WithFileListGetter(profile => profile.EmailClientSettings.AdditionalAttachments)
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
            var interaction = new EditEmailTextInteraction(EmailClientSettings.Subject, EmailClientSettings.Content, EmailClientSettings.AddSignature, EmailClientSettings.Html);

            _interactionRequest.Raise(interaction, EditEmailTextCallback);
        }

        private void EditEmailTextCallback(EditEmailTextInteraction interaction)
        {
            if (!interaction.Success)
                return;

            EmailClientSettings.AddSignature = interaction.AddSignature;
            EmailClientSettings.Content = interaction.Content;
            EmailClientSettings.Subject = interaction.Subject;
            EmailClientSettings.Html = interaction.Html;
        }
    }
}
