using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    internal class DateTimeToTwoLineStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Combine short time and short date with a new line between them
                string shortTime = dateTime.ToShortTimeString();
                string shortDate = dateTime.ToShortDateString();
                return $"{shortTime}\n{shortDate}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is not intended to handle two-way binding
            throw new NotImplementedException();
        }
    }
}
