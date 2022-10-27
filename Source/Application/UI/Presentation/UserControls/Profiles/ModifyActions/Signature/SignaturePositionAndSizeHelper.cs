using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Converter;
using System.Text;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SignaturePositionForUi
    {
        public SignaturePositionForUi(float x, float y, float height, float width)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public interface ISignaturePositionAndSizeHelper
    {
        SignaturePositionForUi GetSignaturePositionForUi(Conversion.Settings.Signature signature, UnitOfMeasurement unit);

        void ApplySignatureForSettings(Conversion.Settings.Signature targetSignature, SignaturePositionForUi signaturePositionFromUi, UnitOfMeasurement unit);

        string GetSignaturePositionAndSizeText(Conversion.Settings.Signature signature, UnitOfMeasurement unit, SignatureTranslation translation);
    }

    public class SignaturePositionAndSizeHelper : ISignaturePositionAndSizeHelper
    {
        private readonly IPositionToUnitConverterFactory _positionToUnitConverterFactory;

        public SignaturePositionAndSizeHelper(IPositionToUnitConverterFactory positionToUnitConverterFactory)
        {
            _positionToUnitConverterFactory = positionToUnitConverterFactory;
        }

        public SignaturePositionForUi GetSignaturePositionForUi(Conversion.Settings.Signature signature, UnitOfMeasurement unit)
        {
            var unitConverter = _positionToUnitConverterFactory.CreatePositionToUnitConverter(unit);
            var x = unitConverter.ConvertBack(signature.LeftX);
            var y = unitConverter.ConvertBack(signature.LeftY);
            var width = unitConverter.ConvertBack(signature.RightX - signature.LeftX);
            var height = unitConverter.ConvertBack(signature.RightY - signature.LeftY);

            return new SignaturePositionForUi(x, y, height, width);
        }

        public void ApplySignatureForSettings(Conversion.Settings.Signature targetSignature, SignaturePositionForUi signaturePositionFromUi, UnitOfMeasurement unit)
        {
            var unitConverter = _positionToUnitConverterFactory.CreatePositionToUnitConverter(unit);

            targetSignature.LeftX = unitConverter.ConvertToUnit(signaturePositionFromUi.X);
            targetSignature.RightX = targetSignature.LeftX + unitConverter.ConvertToUnit(signaturePositionFromUi.Width);
            targetSignature.LeftY = unitConverter.ConvertToUnit(signaturePositionFromUi.Y);
            targetSignature.RightY = targetSignature.LeftY + unitConverter.ConvertToUnit(signaturePositionFromUi.Height);
        }

        public string GetSignaturePositionAndSizeText(Conversion.Settings.Signature signature, UnitOfMeasurement unit, SignatureTranslation translation)
        {
            var signatureForUi = GetSignaturePositionForUi(signature, unit);

            var unitString = unit == UnitOfMeasurement.Centimeter ? "cm" : "\"";
            var signaturePositionAndSize = new StringBuilder(translation.FromLeftLabel);
            signaturePositionAndSize.Append(" ");
            signaturePositionAndSize.Append(signatureForUi.X.ToString("0.00"));
            signaturePositionAndSize.Append(unitString);
            signaturePositionAndSize.Append("   ");
            signaturePositionAndSize.Append(translation.FromBottomLabel);
            signaturePositionAndSize.Append(" ");
            signaturePositionAndSize.Append(signatureForUi.Y.ToString("0.00"));
            signaturePositionAndSize.Append(unitString);
            signaturePositionAndSize.Append("   ");
            signaturePositionAndSize.Append(translation.WidthLabel);
            signaturePositionAndSize.Append(" ");
            signaturePositionAndSize.Append(signatureForUi.Width.ToString("0.00"));
            signaturePositionAndSize.Append(unitString);
            signaturePositionAndSize.Append("   ");
            signaturePositionAndSize.Append(translation.HeightLabel);
            signaturePositionAndSize.Append(" ");
            signaturePositionAndSize.Append(signatureForUi.Height.ToString("0.00"));
            signaturePositionAndSize.Append(unitString);

            return signaturePositionAndSize.ToString();
        }
    }
}
