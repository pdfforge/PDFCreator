using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class BoolToShownColorConverter : IValueConverter
    {
        public Brush ColorActive { get; set; }
        public Brush ColorInActive { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? ColorActive : ColorInActive;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter;
        }
    }
}
