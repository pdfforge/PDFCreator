using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class StringIsNullOrWhiteSpaceConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var valueString = value as string;

            return string.IsNullOrWhiteSpace(valueString) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class IsNullConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return value == null ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
