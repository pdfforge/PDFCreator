using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SignaturePositionAndSizeInteraction : IInteraction
    {
        public SignaturePositionAndSizeInteraction(Conversion.Settings.Signature signature, UnitOfMeasurement unitOfMeasurement)
        {
            Signature = signature;
            UnitOfMeasurement = unitOfMeasurement;
        }

        public Conversion.Settings.Signature Signature { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public bool Success { get; set; } = false;
    }
}
