using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken
{
    public class SeparatorToExampleStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var translation = values.First(v => v is UserTokenTranslation) as UserTokenTranslation;
                var separator = (UserTokenSeperator)values.First(v => v is UserTokenSeperator);

                return translation?.GetUserTokenDocumentExample(separator);
            }
            catch
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
