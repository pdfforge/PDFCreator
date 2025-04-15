using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CSScripting;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class PreviewPagesListToFirstBitmapImageConverter : IValueConverter
    {
        private ImagePathToBitmapConverter _imagePathToBitmapConverter;

        public PreviewPagesListToFirstBitmapImageConverter()
        {
            _imagePathToBitmapConverter = new ImagePathToBitmapConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<PreviewPage> previewPages)
            {
                if (previewPages.IsEmpty())
                    return null;

                return _imagePathToBitmapConverter.Convert(previewPages[0].PreviewImagePath, targetType, parameter, culture);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
