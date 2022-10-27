using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public class SigningHelper
    {
        public static string BuildSignatureText(Signature signatureSettings, string signatureCommonName)
        {
            var text = signatureCommonName;
            text += "\n";
            if (!string.IsNullOrWhiteSpace(signatureSettings.SignLocation))
                text += signatureSettings.SignLocation + ", ";

            text += DateTime.Now.ToString("g");

            if (!string.IsNullOrWhiteSpace(signatureSettings.SignReason))
                text += "\n// " + signatureSettings.SignReason;

            return text;
        }
    }
}
