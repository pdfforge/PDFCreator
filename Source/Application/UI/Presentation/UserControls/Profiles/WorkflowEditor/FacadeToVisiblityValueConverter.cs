using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class FacadeToVisibilityValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var facade = values[0] as IActionFacade;

            if (facade != null && typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType))
            {
                return Visibility.Hidden;
            }

            return (Visibility)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FacadeColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var facade = value as IActionFacade;

            if (facade != null && typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType))
            {
                return new SolidColorBrush(Colors.White);
            }

            return new SolidColorBrush(Colors.WhiteSmoke);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
