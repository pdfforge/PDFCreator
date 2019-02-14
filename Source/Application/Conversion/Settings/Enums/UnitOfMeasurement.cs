using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum UnitOfMeasurement
    {
        [Translation("centimeter")]
        Centimeter,
        [Translation("inch")]
        Inch        
    }
}
