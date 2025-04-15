using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using NLog;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    internal class ImagePathToBitmapConverter : IValueConverter
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        /*
        Convert the images to bitmap to not block the images files by the UI Thread
        */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string filePath)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensures file is fully loaded into memory
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Makes it thread-safe
                    return bitmap;
                }
                catch(Exception ex)
                {
                    _logger.Warn(ex, "Could not create bitmap for " + filePath);
                }
            }
            return null; // Return null if the file is invalid or doesn't exist
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
