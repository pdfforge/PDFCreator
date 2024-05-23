using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;
using System.Globalization;
using System.Windows.Data;
using pdfforge.PDFCreator.Core.Services;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    internal class ObjectToObjectOrTranslatableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            // for EnumTranslation<>
            var trans = type.GetProperty("Translation");
            if (trans != null)
                return trans.GetValue(value);
            
            if (value is Language languageValue)
                return languageValue.NativeName;
            
            if (value is ConversionProfileWrapper profileWrapper)
                return profileWrapper.Name;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
