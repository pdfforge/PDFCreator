using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient
{
    public class MailClientTabTranslation : MailBaseTabTranslation
    {
        public string CheckMailClient { get; private set; } = "Check e-mail client";
        public string NoMapiClientFound { get; private set; } = "Could not find MAPI client (e.g. Thunderbird or Outlook).";
        public string CheckMailClientButtonText { get; private set; } = "Check E-mail Client";
        public string EditEmailButtonText { get; private set; } = "Edit E-mail Text";
        public string MultipleRecipientsHintText { get; private set; } = "Hint: Multiple recipients can be separated by commas";
        public string RecipientsToText { get; private set; } = "To:";
        public string RecipientsCcText { get; private set; } = "CC:";
        public string RecipientsBccText { get; private set; } = "BCC:";
        public string Email { get; set; } = "E-mail";
    }
}
