using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class PositionToUnitConverterFactory : IPositionToUnitConverterFactory
    {
        public IPositionToUnitConverter CreatePositionToUnitConverter(UnitOfMeasurement unit)
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

    public class InchToUnitConverter : IPositionToUnitConverter
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

    public class CentimeterUnitConverter : IPositionToUnitConverter
    {
        public float ConvertToUnit(float value)
        {
            return value * 28.3465f;
        }

        public float ConvertBack(float value)
        {
            return value / 28.3465f;
        }
    }

    public interface IPositionToUnitConverter
    {
        float ConvertToUnit(float value);

        float ConvertBack(float value);
    }

    public interface IPositionToUnitConverterFactory
    {
        IPositionToUnitConverter CreatePositionToUnitConverter(UnitOfMeasurement unit);
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
