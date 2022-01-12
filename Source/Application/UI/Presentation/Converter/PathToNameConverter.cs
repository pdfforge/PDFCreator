using System;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Data;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class PathToNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return null;
            return PathSafe.GetFileName((string) value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
