using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public interface IFontPathHelper
    {
        bool GetFontPath(string fontFile, out string fontPath);
    }

    public class FontPathHelper : IFontPathHelper
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFile _file;

        public FontPathHelper(IFile file)
        {
            _file = file;
        }

        public bool GetFontPath(string fontFile, out string fontPath)
        {
            var globalFontFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            _logger.Trace("Global font folder: " + globalFontFolder);

            fontPath = PathSafe.Combine(globalFontFolder, fontFile);
            if (!_file.Exists(fontPath))
            {
                var userFontFolder = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Microsoft\Windows\Fonts");
                _logger.Trace("User font folder: " + userFontFolder);

                fontPath = PathSafe.Combine(userFontFolder, fontFile);
                if (!_file.Exists(fontPath))
                {
                    _logger.Error($"Font file not found: {fontFile}");
                    return false;
                }
            }

            _logger.Debug("Font path: " + fontPath);

            return true;
        }
    }
}
