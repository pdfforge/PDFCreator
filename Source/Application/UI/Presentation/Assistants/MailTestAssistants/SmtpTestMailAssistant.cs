using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.Utilities;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface ISmtpTest
    {
        Task SendTestMail(EmailSmtpSettings emailSmtpSettings, Accounts accounts);
    }

    public class SmtpTestMailAssistant : TestMailAssistantBase<EmailSmtpSettings>, ISmtpTest
    {
        public SmtpTestMailAssistant(ITranslationUpdater translationUpdater, IInteractionRequest interactionRequest,
            ISmtpMailAction smtpMailAction, ErrorCodeInterpreter errorCodeInterpreter, ITokenHelper tokenHelper,
            ITestFileDummyHelper testFileDummyHelper)
            : base(translationUpdater, tokenHelper, testFileDummyHelper, smtpMailAction, errorCodeInterpreter, interactionRequest)
        { }

        public async Task SendTestMail(EmailSmtpSettings emailSmtpSettings, Accounts accounts)
        {
            await SendTestEmail(emailSmtpSettings, accounts);
        }

        protected override async Task<bool> TrySetJobPasswords(Job job)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(job.Profile);
            if (smtpAccount == null) //The account must be checked, since it is executed before the Action.Check...
            {
                var message = new MessageInteraction(Translation.NoAccount, Translation.SendTestMail, MessageOptions.OK, MessageIcon.Error);
                InteractionRequest.Raise(message);
                return false;
            }

            job.Passwords.SmtpPassword = smtpAccount.Password;

            if (!string.IsNullOrWhiteSpace(job.Passwords.SmtpPassword))
                return true;

            var recipientsString = GetRecipientsString(job.Profile.EmailSmtpSettings);
            var title = Translation.SetSmtpServerPassword;
            var description = Translation.SmtpServerPasswordLabel;
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.None, title, description, false);
            interaction.IntroText = recipientsString;

            await InteractionRequest.RaiseAsync(interaction);

            if (interaction.Result != PasswordResult.StorePassword)
                return false;

            job.Passwords.SmtpPassword = interaction.Password;
            return true;
        }

        protected override void SetMailActionSettings(ConversionProfile profile, EmailSmtpSettings mailActionSettings)
        {
            profile.EmailSmtpSettings = mailActionSettings;
        }

        protected override void ShowSuccess(Job job)
        {
            var title = Translation.SendTestMail;
            var message = Translation.TestMailSent + "\n" + GetRecipientsString(job.Profile.EmailSmtpSettings);
            var interaction = new MessageInteraction(message, title, MessageOptions.OK, MessageIcon.Info);
            InteractionRequest.Raise(interaction);
        }

        private string GetRecipientsString(EmailSmtpSettings smtpSettings)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Translation.RecipientsToText} {smtpSettings.Recipients}");

            if (!string.IsNullOrWhiteSpace(smtpSettings.RecipientsCc))
                sb.AppendLine($"{Translation.RecipientsCcText} {smtpSettings.RecipientsCc}");

            if (!string.IsNullOrWhiteSpace(smtpSettings.RecipientsBcc))
                sb.AppendLine($"{Translation.RecipientsBccText} {smtpSettings.RecipientsBcc}");

            return sb.ToString();
        }
    }
}
