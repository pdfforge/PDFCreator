using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    public class TranslatableFileTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var translatable = value as ITranslatable;
            if (translatable == null)
                throw new Exception("Invalid Binding Path: Given Binding Path must be of type ITranslatable.");

            var converterParameter = parameter?.ToString();
            if (string.IsNullOrWhiteSpace(converterParameter))
                throw new Exception("Invalid converter parameter: Parameter must be of type string");

            var extension = Path.GetExtension(converterParameter);
            var fileType = extension switch
            {
                ".bmp" or ".jpg" or ".gif" or ".png" or ".tif" or ".tiff" => "Image",
                ".mp4" or ".mov" or ".avi" or ".wvm" or ".webm" or ".flv" => "Video",
                _ => ""
            };
            var translationString = translatable.GetType().GetProperty(fileType)?.GetValue(value) as string;
            if (string.IsNullOrWhiteSpace(translationString))
                throw new Exception($"Unknown translation: Translatable '{translatable.GetType().Name}' does not contain translation for : '{converterParameter}'");

            return translationString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
