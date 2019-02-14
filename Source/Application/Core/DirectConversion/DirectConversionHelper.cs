using NLog;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversionHelper
    {
        bool CanConvertDirectly(string file);

        int GetNumberOfPages(string file);
    }

    public class DirectConversionHelper : IDirectConversionHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPdfProcessor _pdfProcessor;
        private readonly IFile _file;

        public DirectConversionHelper(IPdfProcessor pdfProcessor, IFile file)
        {
            _pdfProcessor = pdfProcessor;
            _file = file;
        }

        public bool CanConvertDirectly(string file)
        {
            return IsPsFile(file) || IsPdfFile(file);
        }

        private bool IsPsFile(string file)
        {
            return file.EndsWith(".ps", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsPdfFile(string file)
        {
            return file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetNumberOfPages(string file)
        {
            if (IsPdfFile(file))
                return _pdfProcessor.GetNumberOfPages(file);

            return GetNumberOfPsPages(file);
        }

        private int GetNumberOfPsPages(string fileName)
        {
            var count = 0;
            try
            {
                using (var fs = _file.OpenRead(fileName))
                using (var sr = new System.IO.StreamReader(fs.StreamInstance))
                {
                    while (sr.Peek() >= 0)
                    {
                        var readLine = sr.ReadLine();
                        if (readLine != null && readLine.Contains("%%Page:"))
                            count++;
                    }
                }
            }
            catch
            {
                Logger.Warn("Error while retrieving page count. Set value to 1.");
            }

            return count == 0 ? 1 : count;
        }
    }
}
