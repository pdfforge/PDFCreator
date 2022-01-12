using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Converter;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimePositionUnitConverterFactory : IPositionToUnitConverterFactory
    {
        public IPositionToUnitConverter CreatePositionToUnitConverter(UnitOfMeasurement unit)
        {
            return new CentimeterUnitConverter();
        }
    }
}
