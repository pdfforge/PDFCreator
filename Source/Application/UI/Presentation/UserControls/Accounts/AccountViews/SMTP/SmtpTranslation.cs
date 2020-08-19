using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class SmtpTranslation : AccountsTranslation, IMailBaseTabTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        //ProfileSubTab
        public string SmtpSubTabTitle { get; private set; } = "SMTP";

        //SmtpPasswordStep
        public string SmtpPasswordOverlayTitle { get; private set; } = "SMTP Mail";

        public string SmtpAccountLabel { get; private set; } = "SMTP Account:";
        public string SmtpServerPasswordLabel { get; private set; } = "SMTP Server Password:";

        //Edit Command
        public string EditSmtpAccount { get; private set; } = "Edit SMTP Account";

        //Remove Command
        public string RemoveSmtpAccount { get; private set; } = "Remove SMTP Account";

        private string[] SmtpGetsDisabled { get; set; } = { "The SMTP action will be disabled for this profile.", "The SMTP action will be disabled for this profiles." };

        public string GetSmtpGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, SmtpGetsDisabled);
        }

        //AccountView
        public string EmailAddressLabel { get; private set; } = "E-mail Address:";

        public string PortLabel { get; private set; } = "Port:";
        public string Ssl { get; private set; } = "SSL";

        //ActionUserControl
        public string SendEmailViaSmtp { get; private set; } = "Send e-mail via SMTP";

        public string SelectSmtpAccountLabel { get; private set; } = "Please select a SMTP account:";
        public string EditMailText { get; private set; } = "Edit E-mail Text";
        public string SendTestMail { get; private set; } = "Send Test E-mail";

        //TestAssistant
        public string AttachmentFile { get; private set; } = "AttachmentFile";

        public string TestMailSent { get; private set; } = "The test e-mail was successfully sent to the following recipients:";
        public string SetSmtpServerPassword { get; private set; } = "Set SMTP Server Password";
        public string NoAccount { get; private set; } = "The specified SMTP account is not configured.";

        public string AttachSignatureText { get; private set; } = "Attach pdfforge signature";
        public string BodyTextLabel { get; private set; } = "_Text:";
        public string SubjectLabel { get; private set; } = "_Subject:";
        public string UseHtml { get; private set; } = "Use Html";

        public string MultipleRecipientsHintText { get; private set; } = "Hint: Multiple recipients can be separated by commas";
        public string RecipientsToText { get; private set; } = "To:";
        public string RecipientsCcText { get; private set; } = "CC:";
        public string RecipientsBccText { get; private set; } = "BCC:";
        public string AdditionalAttachmentsText { get; set; } = "Additional E-mail Attachments:";
        public new string AllFiles { get; set; } = "All files";
        public string SelectAttachmentTitle { get; set; } = "Select attachment file";
    }
}
