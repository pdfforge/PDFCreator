using pdfforge.PDFCreator.UI.Interactions.Enums;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class MessageIconToIconConverter : IValueConverter
    {
        private readonly ResourceDictionary _resourceDictionary;

        public MessageIconToIconConverter()
        {
            _resourceDictionary = new ResourceDictionary { Source = new Uri("PDFCreator.Presentation;component/Styles/Logos.xaml", UriKind.Relative) };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageIcon)
            {
                var icon = (MessageIcon)value;

                var img = new Image();
                switch (icon)
                {
                    case MessageIcon.Error:
                        return GetResourceAsSomething("Error");

                    case MessageIcon.Warning:
                        return GetResourceAsSomething("Warning");

                    case MessageIcon.Exclamation:
                        img.Source = ConvertBitmap(SystemIcons.Exclamation.ToBitmap());
                        return img;

                    case MessageIcon.Question:
                        img.Source = ConvertBitmap(SystemIcons.Question.ToBitmap());
                        return img;

                    case MessageIcon.Info:
                        img.Source = ConvertBitmap(SystemIcons.Information.ToBitmap());
                        return img;

                    case MessageIcon.PDFCreator:
                        return GetResourceAsSomething("PDFCreatorLogo");

                    case MessageIcon.PDFForge:
                        return GetResourceAsSomething("RedFlame");
                }
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private object GetResourceAsSomething(string resourceString)
        {
            return _resourceDictionary[resourceString];
        }

        private BitmapImage ConvertBitmap(Bitmap value)
        {
            var ms = new MemoryStream();
            value.Save(ms, ImageFormat.Png);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }
    }
}
