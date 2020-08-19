using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public interface IFontPathHelper
    {
        ActionResult<string> GetFontPath(ConversionProfile profile);
    }

    public class FontPathHelper : IFontPathHelper
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFile _file;

        public FontPathHelper(IFile file)
        {
            _file = file;
        }

        public ActionResult<string> GetFontPath(ConversionProfile profile)
        {
            var globalFontFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            _logger.Trace("Global font folder: " + globalFontFolder);

            var fontPath = PathSafe.Combine(globalFontFolder, profile.Stamping.FontFile);
            if (!_file.Exists(fontPath))
            {
                var userFontFolder = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Microsoft\Windows\Fonts");
                _logger.Trace("User font folder: " + userFontFolder);

                fontPath = PathSafe.Combine(userFontFolder, profile.Stamping.FontFile);
                if (!_file.Exists(fontPath))
                {
                    _logger.Error($"Font file not found: {profile.Stamping.FontFile}");
                    return new ActionResult<string>(ErrorCode.Stamp_FontNotFound);
                }
            }

            _logger.Debug("Font path: " + fontPath);

            return new ActionResult<string>(fontPath);
        }
    }
}
