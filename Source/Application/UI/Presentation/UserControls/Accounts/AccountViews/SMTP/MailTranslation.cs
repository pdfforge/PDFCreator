using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class SmtpMailTranslation : MailTranslation
    {
        public override string Title { get; set; } = "Email (SMTP)";
        public override string InfoText { get; set; } = "Sends the document as an email without user interaction by using SMTP.";
    }

    public class OutlookWebTranslation : AccountsTranslation, IActionTranslation
    {
        public string MultipleRecipientsHintText { get; set; } = "Hint: Multiple recipients can be separated by commas";
        public string BlankToField { get; set; } = "Blank 'To' field";
        public string RecipientsToText { get; set; } = "To:";
        public string RecipientsCcText { get; set; } = "CC:";
        public string RecipientsBccText { get; set; } = "BCC:";
        public string EditMailText { get; set; } = "Edit subject and content";

        public string MailAttachmentTitle { get; set; } = "Select email attachment file";

        public string Email { get; set; } = "Email via Outlook Web";
        public virtual string Title { get; set; } = "Email (Outlook Web)";
        public virtual string InfoText { get; set; } = "Creates a draft in Outlook Web Access (OWA) with the converted documents attached and opens the browser to edit and send it.";

        public virtual string AutomaticSend { get; set; } = "Send email directly";
        public virtual string ShowDraft { get; set; } = "Open draft in browser";
        public virtual string SaveDraft { get; set; } = "Create draft";
        public virtual string AddAttachmentFile { get; set; } = "Add attachment";
    }

    public class MailTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        //Password Overlay
        public string SmtpServerPasswordLabel { get; set; } = "SMTP server password:";

        //Edit Command
        public string EditSmtpAccount { get; set; } = "Edit SMTP account";

        //Remove Command
        public string RemoveSmtpAccount { get; set; } = "Remove SMTP Account";

        private string[] SmtpGetsDisabled { get; set; } = { "The SMTP action will be disabled for this profile.", "The SMTP action will be disabled for this profiles." };

        public string GetSmtpGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, SmtpGetsDisabled);
        }

        //AccountView
        public string EmailAddressLabel { get; set; } = "Email address:";

        public string PortLabel { get; set; } = "Port:";
        public string Ssl { get; set; } = "SSL/TLS";

        //ActionUserControl
        public string SendEmailViaSmtp { get; set; } = "Send email via SMTP";

        public string SelectSmtpAccountLabel { get; set; } = "Please select a SMTP account:";
        public string SendTestMail { get; set; } = "Send test email";

        //TestAssistant
        public string DocumentFile { get; set; } = "Document File";

        public virtual string AddAttachmentFile { get; set; } = "Add attachment";
        public string EditMailText { get; set; } = "Edit subject and content";

        public string TestMailSent { get; set; } = "The test email was successfully sent to the following recipients:";
        public string SetSmtpServerPassword { get; set; } = "Set SMTP server password";
        public string NoAccount { get; set; } = "The specified account is not configured.";

        public string AttachSignatureText { get; set; } = "Attach pdfforge signature";
        public string BodyTextLabel { get; set; } = "_Text:";
        public string SubjectLabel { get; set; } = "_Subject:";
        public string UseHtml { get; set; } = "Use HTML";
        public string FormatLabel { get; set; } = "Format:";

        public EnumTranslation<EmailFormatSetting>[] EmailFormatValues { get; set; } = EnumTranslation<EmailFormatSetting>.CreateDefaultEnumTranslation();

        public string MultipleRecipientsHintText { get; set; } = "Hint: Multiple recipients can be separated by commas";

        public string BlankToField { get; set; } = "Blank 'To' field";
        public string RecipientsToText { get; set; } = "To:";
        public string RecipientsCcText { get; set; } = "CC:";
        public string RecipientsBccText { get; set; } = "BCC:";
        public string AdditionalAttachmentsText { get; set; } = "Additional Email Attachments:";

        public string MailAttachmentTitle { get; set; } = "Select email attachment file";

        public string CheckMailClient { get; set; } = "Check email client";
        public string Email { get; set; } = "Open email in installed email client";
        public virtual string Title { get; set; } = "Email (Desktop)";
        public virtual string InfoText { get; set; } = "Opens the default email client with the converted document as attachment.";

        public string SpecifySender { get; set; } = "Specify sender";
        public string SpecifySenderLabel { get; set; } = "Specify sender:";
        public string OnBehalfOfLabel { get; set; } = "On behalf of:";
        public string OnBehalfOf { get; set; } = "on behalf of";
        public string DifferingFromToolTip { get; set; } = "If set it will be used as from and the address from the account will be set as sender";
        public string DisplayName { get; set; } = "Display name:";
        public string ReplyTo { get; set; } = "Reply to:";
        public string Back { get; set; } = "Back";
    }
}
