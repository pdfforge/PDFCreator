using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class MenuTabColumnWidthConverter : IValueConverter
    {
        public int TrueValue { get; set; }
        public int FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool b))
                return null;
            return b ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
