using System;
using System.Drawing.Imaging;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using PdfiumViewer;
using SystemInterface.IO;
using Logger = NLog.Logger;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using System.Threading;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IPdfToPreviewConverter
    {
        Task<PreviewPages> GeneratePreviewPages(string pdfFilePath, CancellationToken cancellationToken);
    }

    public class PdfToPreviewConverter : IPdfToPreviewConverter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const int MaxImageSize = 92; // from PreviewControl


        private readonly IDirectory _directory;
        private readonly IGuid _guid;

        private readonly string _tempPreviewFolder;


        public PdfToPreviewConverter(IDirectory directory, ITempFolderProvider tempFolderProvider, IGuid guid)
        {
            _directory = directory;
            _guid = guid;
            _tempPreviewFolder = PathSafe.Combine(tempFolderProvider.TempFolder, "Preview");
        }

        public async Task<PreviewPages> GeneratePreviewPages(string pdfFilePath, CancellationToken cancellationToken)
        {
            return await Task.Run(() => DoGeneratePreviewPages(pdfFilePath, cancellationToken));
        }

        private PreviewPages DoGeneratePreviewPages(string pdfFilePath, CancellationToken cancellationToken)
        {
            var previewDirectory = PathSafe.Combine(_tempPreviewFolder, _guid.NewGuidString());
            var previewPages = new PreviewPages(previewDirectory);
            var sourceFileNameWithoutExtension = PathSafe.GetFileNameWithoutExtension(pdfFilePath);
            var previewImagePathBase = PathSafe.Combine(previewDirectory, sourceFileNameWithoutExtension);

            try
            {
                _directory.CreateDirectory(previewDirectory);

                if (cancellationToken.IsCancellationRequested)
                    return previewPages;

                using var document = PdfDocument.Load(pdfFilePath);

                for (var pageIndex = 0; pageIndex < document.PageCount; pageIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var imagePath = $"{previewImagePathBase}_{pageIndex + 1}.jpeg";

                    // Longest side becomes max size and the shorter side is calculated relatively 
                    var pageSize = document.PageSizes[pageIndex];
                    int width, height;
                    if (pageSize.Height > pageSize.Width) // Portrait
                    {
                        height = MaxImageSize;
                        width = (int)Math.Round(pageSize.Width * MaxImageSize / pageSize.Height);
                    }
                    else // Landscape
                    {
                        width = MaxImageSize;
                        height = (int)Math.Round(pageSize.Height * MaxImageSize / pageSize.Width);
                    }

                    using var image = document.Render(pageIndex, width, height, 96, 96,  PdfRenderFlags.Annotations);
                    image.Save(imagePath, ImageFormat.Jpeg);

                    var previewPage = new PreviewPage(pageIndex + 1, imagePath);
                    previewPages.PreviewPageList.Add(previewPage);
                    //Task.Delay(TimeSpan.FromMilliseconds(5000)).GetAwaiter().GetResult(); // Uncomment for testing
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not create preview for " + pdfFilePath);
                previewPages.PreviewPageList.Clear();
            }

            return previewPages;
        }
    }
}