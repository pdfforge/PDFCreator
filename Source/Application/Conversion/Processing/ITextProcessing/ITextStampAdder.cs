using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Properties;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextStampAdder
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly FontPathHelper _fontPathHelper;

        public ITextStampAdder(IFile file)
        {
            _fontPathHelper = new FontPathHelper(file);
        }

        /// <summary>
        ///     Add a stamp (set in profile) to a document, that is opened in the stamper.
        ///     The function does nothing, if stamp settings are disabled.
        /// </summary>
        /// <param name="stamper">the document that needs to be stamped/param>
        /// <param name="profile">Profile with stamp settings</param>
        /// <exception cref="ProcessingException">In case of any error.</exception>
        internal void AddStamp(PdfDocument document, ConversionProfile profile)
        {
            try
            {
                _logger.Debug("Start adding stamp.");
                var result = _fontPathHelper.GetFontPath(profile);
                if (!result)
                    throw new ProcessingException("Error during font path detection.", result[0]);

                var fontPath = result.Value;
                DoAddStamp(document, profile, fontPath);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType() + " while adding stamp.";
                throw new ProcessingException(errorMessage, ErrorCode.Stamp_GenericError, ex);
            }
        }

        private void DoAddStamp(PdfDocument pdfDocument, ConversionProfile profile, string fontPath)
        {
            var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, true);
            var color = new DeviceRgb(profile.Stamping.Color);

            pdfDocument.AddFont(font);

            var numberOfPages = pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= numberOfPages; i++)
            {
                var pdfImportedPage = pdfDocument.GetPage(i);

                var pageSize = pdfImportedPage.GetPageSize();
                float rotationAngle = (float)(Math.Atan2(pageSize.GetHeight(),
                                                  pageSize.GetWidth()));

                var pdfPage = pdfDocument.GetPage(i);
                var layer = pdfPage.NewContentStreamAfter();

                var canvas = new PdfCanvas(layer, pdfPage.GetResources(), pdfDocument);
                canvas.SaveState();
                var canvasLayer = new Canvas(canvas, pdfDocument, pdfPage.GetPageSize());

                if (profile.Stamping.FontAsOutline)
                {
                    canvasLayer.SetStrokeWidth(profile.Stamping.FontOutlineWidth);
                    canvasLayer.SetStrokeColor(color);
                    canvasLayer.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
                }

                canvasLayer.SetFontColor(color);
                canvasLayer.SetFont(font);
                canvasLayer.SetFontSize(profile.Stamping.FontSize);

                canvasLayer.ShowTextAligned(
                    profile.Stamping.StampText,
                    pdfPage.GetPageSize().GetWidth() / 2,
                    pdfPage.GetPageSize().GetHeight() / 2,
                    TextAlignment.CENTER,
                    VerticalAlignment.MIDDLE,
                    rotationAngle);

                canvas.RestoreState();
            }
        }
    }
}
