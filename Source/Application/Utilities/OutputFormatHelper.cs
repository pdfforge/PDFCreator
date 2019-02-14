using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public class OutputFormatHelper
    {
        public bool HasValidExtension(string file, OutputFormat outputFormat)
        {
            var extension = PathSafe.GetExtension(file);

            if (extension == null)
                return false;

            var validExtensions = GetValidExtensions(outputFormat);

            return validExtensions.Contains(extension.ToLower());
        }

        public bool HasKnownExtension(string path)
        {
            foreach (var outputFormat in Enum.GetValues(typeof(OutputFormat)).Cast<OutputFormat>())
            {
                if (HasValidExtension(path, outputFormat))
                    return true;
            }

            return false;
        }

        public string EnsureValidExtension(string file, OutputFormat outputFormat)
        {
            if (HasValidExtension(file, outputFormat))
                return file;

            var validExtension = GetValidExtensions(outputFormat).First();

            if (!HasKnownExtension(file))
                file = file + validExtension;

            return PathSafe.ChangeExtension(file, validExtension);
        }

        private string[] GetValidExtensions(OutputFormat outputFormat)
        {
            if (outputFormat.IsPdf())
                return new[] { ".pdf" };

            switch (outputFormat)
            {
                case OutputFormat.Jpeg:
                    return new[] { ".jpg", ".jpeg" };

                case OutputFormat.Png:
                    return new[] { ".png" };

                case OutputFormat.Tif:
                    return new[] { ".tif", ".tiff" };

                case OutputFormat.Txt:
                    return new[] { ".txt" };
            }

            throw new NotImplementedException($"OutputFormat '{outputFormat}' is not known to {nameof(OutputFormatHelper)}!");
        }

        public string GetExtension(OutputFormat outputFormat)
        {
            return GetValidExtensions(outputFormat)[0];
        }

        public OutputFormat GetOutputFormatByPath(string path)
        {
            var ext = PathSafe.GetExtension(path);
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        return OutputFormat.Jpeg;

                    case ".png":
                        return OutputFormat.Png;

                    case ".tif":
                    case ".tiff":
                        return OutputFormat.Tif;

                    case ".txt":
                        return OutputFormat.Txt;

                    case ".pdf":
                        return OutputFormat.Pdf;
                }
            }
            throw new NotImplementedException($"OutputFormat '{ext}' is not known to {nameof(OutputFormatHelper)}!");
        }
    }
}
