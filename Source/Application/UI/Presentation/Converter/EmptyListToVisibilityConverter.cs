using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class EmptyListToVisibilityConverter : DependencyObject, IValueConverter
    {
        public Visibility EmptyValue { get; set; }
        public Visibility FilledValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable list)
            {
                if (list.TryGetList().Count > 0)
                {
                    return FilledValue;
                }
            }
            return EmptyValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
