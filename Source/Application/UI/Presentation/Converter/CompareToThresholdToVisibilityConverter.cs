using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class CompareToThresholdToVisibilityConverter : IValueConverter
    {
        public int Threshold { get; set; } = 0;
        public Visibility BiggerValue { get; set; } = Visibility.Visible;
        public Visibility SmallerOrEqualValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (int)value > Threshold ? BiggerValue : SmallerOrEqualValue;
            }
            catch
            {
                return 0;
            }
        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
