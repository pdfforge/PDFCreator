using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper.Gpo;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class TranslatorConverter : IValueConverter
    {
        private GpoTranslation _translation = new GpoTranslation();

        public TranslatorConverter()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                var translationUpdater = RestrictedServiceLocator.Current.GetInstance<ITranslationUpdater>();
                translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            }
        }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return _translation.SetByAdministrator;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
