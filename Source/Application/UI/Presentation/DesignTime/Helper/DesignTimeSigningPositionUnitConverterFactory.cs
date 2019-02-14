using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Converter;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeSigningPositionUnitConverterFactory : ISigningPositionToUnitConverterFactory
    {
        public ISigningPositionToUnitConverter CreateSigningPositionToUnitConverter(UnitOfMeasurement unit)
        {
            return new CentimeterUnitConverter();
        }
    }
}
