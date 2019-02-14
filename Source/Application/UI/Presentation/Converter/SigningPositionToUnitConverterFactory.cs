using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class SigningPositionToUnitConverterFactory : ISigningPositionToUnitConverterFactory
    {
        public ISigningPositionToUnitConverter CreateSigningPositionToUnitConverter(UnitOfMeasurement unit)
        {
            switch (unit)
            {
                case UnitOfMeasurement.Centimeter:
                    return new CentimeterUnitConverter();

                case UnitOfMeasurement.Inch:
                    return new InchToUnitConverter();

                default:
                    return new CentimeterUnitConverter();
            }
        }
    }

    public class InchToUnitConverter : ISigningPositionToUnitConverter
    {
        public float ConvertToUnit(float value)
        {
            return value * 72;
        }

        public float ConvertBack(float value)
        {
            return value / 72;
        }
    }

    public class CentimeterUnitConverter : ISigningPositionToUnitConverter
    {
        public float ConvertToUnit(float value)
        {
            return value * 28;
        }

        public float ConvertBack(float value)
        {
            return value / 28;
        }
    }

    public interface ISigningPositionToUnitConverter
    {
        float ConvertToUnit(float value);

        float ConvertBack(float value);
    }

    public interface ISigningPositionToUnitConverterFactory
    {
        ISigningPositionToUnitConverter CreateSigningPositionToUnitConverter(UnitOfMeasurement unit);
    }

    //public class SignaturePositionCoordinates
    //{
    //    public float LeftX { get; set; }
    //    public float RightX { get; set; }

    //    public float LeftY { get; set; }
    //    public float RightY { get; set; }

    //    public UnitOfMeasurement Unit { get; set; }
    //}
}
