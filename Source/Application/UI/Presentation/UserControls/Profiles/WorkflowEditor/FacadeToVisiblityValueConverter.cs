using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using pdfforge.PDFCreator.UI.Presentation.Converter;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class HiddenWhenFacadeIsFixedOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var facade = value as IPresenterActionFacade;

            if (facade != null && typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType))
            {
                return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FacadeColorValueConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ReferenceProperty = DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(FacadeColorValueConverter));

        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(ReferenceProperty); }
            set { SetValue(ReferenceProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var facade = value as IPresenterActionFacade;

            if (facade != null && typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType))
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            return Background;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
