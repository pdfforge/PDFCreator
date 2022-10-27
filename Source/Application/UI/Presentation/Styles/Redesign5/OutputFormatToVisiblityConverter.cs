using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.Redesign5
{
    internal class OutputFormatToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var actualFormat = value as string;
            var targetFormat = parameter as string;

            if (actualFormat != null && targetFormat != null && actualFormat.Equals(targetFormat))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
