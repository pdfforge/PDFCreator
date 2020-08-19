using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    /// <summary>
    /// Code largely based on https://stackoverflow.com/a/17433777
    /// </summary>
    public class EllipsisAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            if (parameter == null)
                return Binding.DoNothing;

            if (!int.TryParse(parameter.ToString(), out var subtractionValue))
                return Binding.DoNothing;

            return (double)value - subtractionValue; // subtractValue's effect will differ
                                                     // depending on to which control the EllipsisAdjustmentConverter is bound to
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
