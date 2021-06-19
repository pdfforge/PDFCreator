using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class EditEmailTextInteraction : IInteraction
    {
        public bool OfferOnlyHtmlCheckbox { get; }
        public bool AddSignature { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EmailFormatSetting Format { get; set; }
        public bool Success { get; set; }

        public EditEmailTextInteraction(IMailActionSettings actionSettings)
        {
            Subject = actionSettings.Subject;
            Content = actionSettings.Content;
            AddSignature = actionSettings.AddSignature;
            Format = actionSettings.Format;

            OfferOnlyHtmlCheckbox = actionSettings is EmailSmtpSettings;
        }
    }
}
