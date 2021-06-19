using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class SmtpMailTranslation : MailTranslation
    {
        public override string Title { get; set; } = "E-mail (SMTP)";
        public override string InfoText { get; set; } = "Sends the document as an e-mail without user interaction by using SMTP.";
    }

    public class MailTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        //SmtpPasswordStep
        public string SmtpPasswordOverlayTitle { get; set; } = "SMTP Mail";

        public string SmtpAccountLabel { get; set; } = "SMTP Account:";
        public string SmtpServerPasswordLabel { get; set; } = "SMTP Server Password:";

        //Edit Command
        public string EditSmtpAccount { get; set; } = "Edit SMTP Account";

        //Remove Command
        public string RemoveSmtpAccount { get; set; } = "Remove SMTP Account";

        private string[] SmtpGetsDisabled { get; set; } = { "The SMTP action will be disabled for this profile.", "The SMTP action will be disabled for this profiles." };

        public string GetSmtpGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, SmtpGetsDisabled);
        }

        //AccountView
        public string EmailAddressLabel { get; set; } = "E-mail Address:";

        public string PortLabel { get; set; } = "Port:";
        public string Ssl { get; set; } = "SSL";

        //ActionUserControl
        public string SendEmailViaSmtp { get; set; } = "Send e-mail via SMTP";

        public string SelectSmtpAccountLabel { get; set; } = "Please select a SMTP account:";
        public string EditMailText { get; set; } = "Edit e-mail text";
        public string SendTestMail { get; set; } = "Send test e-mail";

        //TestAssistant
        public string DocumentFile { get; set; } = "Document File";

        public string AdditionalAttachmentFile { get; set; } = "Additional Attachment File";

        public string TestMailSent { get; set; } = "The test e-mail was successfully sent to the following recipients:";
        public string SetSmtpServerPassword { get; set; } = "Set SMTP Server Password";
        public string NoAccount { get; set; } = "The specified account is not configured.";

        public string AttachSignatureText { get; set; } = "Attach pdfforge signature";
        public string BodyTextLabel { get; set; } = "_Text:";
        public string SubjectLabel { get; set; } = "_Subject:";
        public string UseHtml { get; set; } = "Use HTML";
        public string FormatLabel { get; set; } = "Format:";

        public EnumTranslation<EmailFormatSetting>[] EmailFormatValues { get; set; } = EnumTranslation<EmailFormatSetting>.CreateDefaultEnumTranslation();

        public string MultipleRecipientsHintText { get; set; } = "Hint: Multiple recipients can be separated by commas";

        public string Edit { get; set; } = "Edit";

        public string BlankToField { get; set; } = "Blank 'To' field";
        public string RecipientsToText { get; set; } = "To:";
        public string RecipientsCcText { get; set; } = "CC:";
        public string RecipientsBccText { get; set; } = "BCC:";
        public string AdditionalAttachmentsText { get; set; } = "Additional E-mail Attachments:";

        public string MailAttachmentTitle { get; set; } = "Select E-mail Attachment File";

        public string CheckMailClient { get; set; } = "Check e-mail client";
        public string Email { get; set; } = "E-mail";
        public virtual string Title { get; set; } = "E-mail";
        public virtual string InfoText { get; set; } = "Opens the default e-mail client with the converted document as attachment.";

        public string SpecifySender { get; set; } = "Specify Sender";
        public string SpecifySenderLabel { get; set; } = "Specify Sender:";
        public string OnBehalfOfLabel { get; set; } = "On Behalf Of:";
        public string OnBehalfOf { get; set; } = "on behalf of";
        public string DifferingFromToolTip { get; set; } = "If set it will be used as From and the address from the account will be set as Sender";
        public string DisplayName { get; set; } = "Display Name:";
        public string ReplyTo { get; set; } = "Reply To:";
        public string Back { get; set; } = "Back";
    }
}
