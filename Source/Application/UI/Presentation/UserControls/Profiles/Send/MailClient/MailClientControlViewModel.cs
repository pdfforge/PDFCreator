using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient
{
    public class MailClientControlViewModel : MailBaseControlViewModel<MailClientTabTranslation>
    {
        private readonly IClientTestEmail _clientTestEmail;
        private readonly IInteractionRequest _interactionRequest;

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }

        public DelegateCommand EmailClientTestCommand { get; set; }
        public DelegateCommand EditEmailTextCommand { get; set; }
        private EmailClientSettings EmailClientSettings => CurrentProfile?.EmailClientSettings;

        public MailClientControlViewModel(IInteractionRequest interactionRequest,
            IClientTestEmail clientTestEmail,
            ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IOpenFileInteractionHelper openFileInteractionHelper)
            : base(translationUpdater, selectedProfileProvider, dispatcher, openFileInteractionHelper)
        {
            _interactionRequest = interactionRequest;
            _clientTestEmail = clientTestEmail;

            CreateTokenViewModels(tokenViewModelFactory);

            EmailClientTestCommand = new DelegateCommand(EmailClientTestExecute);
            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);
            RemoveSelectedFromListCommand = new DelegateCommand(RemoveSelectedFromList);
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

            AdditionalAttachmentsTokenViewModel = builder
                .WithSelector(p => AdditionalAttachmentsForTextBox)
                .WithButtonCommand(SelectAttachmentFileAction)
                .WithSecondaryButtonCommand(AddFilePathToAdditionalAttachments)
                .Build();
        }

        public override void MountView()
        {
            RecipientsTokenViewModel.MountView();
            RecipientsCcTokenViewModel.MountView();
            RecipientsBccTokenViewModel.MountView();
            AdditionalAttachmentsTokenViewModel.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            RecipientsTokenViewModel.UnmountView();
            RecipientsCcTokenViewModel.UnmountView();
            RecipientsBccTokenViewModel.UnmountView();
            AdditionalAttachmentsTokenViewModel.UnmountView();
        }

        private void EmailClientTestExecute(object obj)
        {
            var success = _clientTestEmail.SendTestEmail(EmailClientSettings);
            if (!success)
                DisplayNoClientFoundMessage();
        }

        protected override IMailActionSettings MailActionSettings => EmailClientSettings;

        private void DisplayNoClientFoundMessage()
        {
            var caption = Translation.CheckMailClient;
            var message = Translation.NoMapiClientFound;

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Warning);
            _interactionRequest.Raise(interaction);
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
