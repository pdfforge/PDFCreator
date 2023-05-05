using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public class OfflineActivationViewModelTranslation : ITranslatable
    {
        public string InvalidLicenseKeySyntax { get; private set; } = "Wrong license key syntax.\nIt must look like '12345-67890-ABCDE-EFGHI-JKLMNO-PRSTU'.\nValid characters are: A-Z, 0-9 and the dash.";
        public string CancelButtonContent { get; private set; } = "_Cancel";
        public string LicenseKeyLabelContent { get; private set; } = "Please enter your license key:";
        public string CopyTheOfflineActivationString { get; private set; } = "Copy the offline activation string:";
        public string OkButtonContent { get; private set; } = "_Ok";
        public string PasteResponseLabelContent { get; private set; } = "Paste the response code here:";
        public string RequestActivationCodeLabelContent { get; private set; } = "Request an activation code on our website:";
        public string Title { get; private set; } = "Offline activation";
    }
}
