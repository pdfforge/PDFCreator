using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    /// <summary>
    /// Does no conversion at all, but can be used to debug if bindings work as expected
    /// </summary>
    public class IntValueConverter : IValueConverter
    {
        public int Offset { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Int32.Parse(value.ToString()) - Offset;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Int32.Parse(value.ToString()) + Offset;

            return value;
        }
    }
}
