using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface IDefaultViewerCheck
    {
        ActionResult Check(DefaultViewer defaultViewer);
    }

    public class DefaultViewerCheck : IDefaultViewerCheck
    {
        private readonly IFile _file;

        public DefaultViewerCheck(IFile file)
        {
            _file = file;
        }

        public ActionResult Check(DefaultViewer defaultViewer)
        {
            var result = new ActionResult();

            if (!defaultViewer.IsActive)
                return result;

            if (string.IsNullOrEmpty(defaultViewer.Path))
                AddMissingPathError(result, defaultViewer.OutputFormat);
            else if (!_file.Exists(defaultViewer.Path))
                AddNotExistingFileError(result, defaultViewer.OutputFormat);

            return result;
        }

        private void AddMissingPathError(ActionResult result, OutputFormat format)
        {
            if (format.IsPdf())
                result.Add(ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf);
            switch (format)
            {
                case OutputFormat.Jpeg:
                    result.Add(ErrorCode.DefaultViewer_PathIsEmpty_for_Jpeg);
                    break;

                case OutputFormat.Png:
                    result.Add(ErrorCode.DefaultViewer_PathIsEmpty_for_Png);
                    break;

                case OutputFormat.Tif:
                    result.Add(ErrorCode.DefaultViewer_PathIsEmpty_for_Tif);
                    break;

                case OutputFormat.Txt:
                    result.Add(ErrorCode.DefaultViewer_PathIsEmpty_for_Txt);
                    break;

                default:
                    break;
            }
        }

        private void AddNotExistingFileError(ActionResult result, OutputFormat format)
        {
            if (format.IsPdf())
                result.Add(ErrorCode.DefaultViewer_FileDoesNotExist_For_Pdf);

            switch (format)
            {
                case OutputFormat.Jpeg:
                    result.Add(ErrorCode.DefaultViewer_FileDoesNotExist_For_Jpeg);
                    break;

                case OutputFormat.Png:
                    result.Add(ErrorCode.DefaultViewer_FileDoesNotExist_For_Png);
                    break;

                case OutputFormat.Tif:
                    result.Add(ErrorCode.DefaultViewer_FileDoesNotExist_For_Tif);
                    break;

                case OutputFormat.Txt:
                    result.Add(ErrorCode.DefaultViewer_FileDoesNotExist_For_Txt);
                    break;

                default:
                    break;
            }
        }
    }
}
