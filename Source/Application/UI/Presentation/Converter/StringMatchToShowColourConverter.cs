using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class StringMatchToShowColourConverter : IValueConverter
    {
        public Brush ColourActive { get; set; }
        public Brush ColourInActive { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString().Equals(parameter))
            {
                return ColourActive;
            }
            return ColourInActive;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
