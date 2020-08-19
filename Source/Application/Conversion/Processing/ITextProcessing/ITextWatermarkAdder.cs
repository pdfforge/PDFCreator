using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using Path = System.IO.Path;
using Rectangle = iText.Kernel.Geom.Rectangle;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    internal class ITextWatermarkAdder
    {
        //ActionId = 17;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Add a backgroundfile (setted in profile) to a document, that is opend in the stamper.
        ///     It considers cover- and attachment pages (that can also request a background).
        ///     The function does nothing, if background settings are disabled.
        /// </summary>
        /// <param name="stamper">Stamper with document</param>
        /// <param name="profile">Profile with backgroundpage settings</param>
        /// <exception cref="ProcessingException">In case of any error.</exception>
        internal void AddBackground(PdfDocument pdfDocument, ConversionProfile profile)
        {
            try
            {
                _logger.Debug("Start adding background.");
                DoAddWatermark(pdfDocument, profile.BackgroundPage, GetPdfCanvasForBack);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType() + " while adding background.";
                if (ex is BadPasswordException)
                    throw new ProcessingException(errorMessage, ErrorCode.Background_BadPasswordError, ex);

                throw new ProcessingException(errorMessage, ErrorCode.Background_GenericError, ex);
            }
        }

        internal void AddForeground(PdfDocument pdfDocument, ConversionProfile profile)
        {
            try
            {
                _logger.Debug("Start adding watermark.");
                DoAddWatermark(pdfDocument, profile.Watermark, GetPdfCanvasForFront);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType() + " while adding watermark.";
                if (ex is BadPasswordException)
                    throw new ProcessingException(errorMessage, ErrorCode.Watermark_BadPassword, ex);

                throw new ProcessingException(errorMessage, ErrorCode.Watermark_GenericError, ex);
            }
        }

        private PdfCanvas GetPdfCanvasForBack(PdfPage pdfPage, PdfDocument pdfDocument)
        {
            return new PdfCanvas(pdfPage.NewContentStreamBefore(), pdfPage.GetResources(), pdfDocument);
        }

        private PdfCanvas GetPdfCanvasForFront(PdfPage pdfPage, PdfDocument pdfDocument)
        {
            return new PdfCanvas(pdfPage.NewContentStreamAfter(), pdfPage.GetResources(), pdfDocument);
        }

        private void DoAddWatermark(PdfDocument pdfDocument, IWatermarkSettings watermarkSettings, Func<PdfPage, PdfDocument, PdfCanvas> GetPdfCanvas)
        {
            if (!watermarkSettings.Enabled)
                return;

            _logger.Debug("File: " + Path.GetFullPath(watermarkSettings.File));

            var nDocument = pdfDocument.GetNumberOfPages();

            var watermarkReader = new PdfReader(watermarkSettings.File);
            var watermarkDocument = new PdfDocument(watermarkReader);

            var nWatermark = watermarkDocument.GetNumberOfPages();
            var watermarkPageMapper = new WatermarkPageMapper(nWatermark, watermarkSettings);

            for (var documentPageNumber = 1; documentPageNumber <= nDocument; documentPageNumber++)
            {
                if (!watermarkPageMapper.GetWatermarkPageNumber(documentPageNumber, out var watermarkPageNumber))
                    continue;

                var watermarkPage = watermarkDocument.GetPage(watermarkPageNumber);
                var documentPage = pdfDocument.GetPage(documentPageNumber);

                var watermarkPageSize = watermarkPage.GetPageSize();
                var watermarkPageRotation = watermarkPage.GetRotation();
                var documentPageSize = documentPage.GetPageSize();

                if (documentPage.GetRotation() == 90 || documentPage.GetRotation() == 270)
                {
                    //Turn with document page...
                    //*
                    watermarkPageRotation += 90;
                    watermarkPageRotation = watermarkPageRotation % 360;
                    //*/
                    documentPageSize = new Rectangle(documentPageSize.GetHeight(), documentPageSize.GetWidth());
                }

                var pdfCanvas = GetPdfCanvas(documentPage, pdfDocument);

                pdfCanvas.SaveState();
                var state = new PdfExtGState();
                var opacity = watermarkSettings.Opacity / 100.0f;
                state.SetFillOpacity(opacity);
                pdfCanvas.SetExtGState(state);

                var watermarkCopy = watermarkPage.CopyTo(pdfDocument);
                var watermarkXObject = new PdfFormXObject(watermarkCopy);

                AddPageWithRotationAndScaling(pdfCanvas, documentPageSize, watermarkXObject, watermarkPageSize, watermarkPageRotation, watermarkSettings.FitToPage);

                pdfCanvas.RestoreState();
            }

            watermarkReader.Close();
        }

        private void AddPageWithRotationAndScaling(PdfCanvas pdfCanvas, Rectangle documentPageSize, PdfXObject watermarkXObject, Rectangle watermarkPageSize, int rotation, bool fitToPage)
        {
            float scaleWidth;
            float scaleHeight;
            float scale;
            float backgroundHeight;
            float backgroundWidth;

            switch (rotation)
            {
                case 90:
                    scaleWidth = documentPageSize.GetWidth() / watermarkPageSize.GetHeight();
                    scaleHeight = documentPageSize.GetHeight() / watermarkPageSize.GetWidth();
                    scale = scaleWidth < scaleHeight ? scaleWidth : scaleHeight;
                    scale = fitToPage ? scale : 1;

                    backgroundHeight = scale * watermarkPageSize.GetHeight();
                    backgroundWidth = scale * watermarkPageSize.GetWidth();

                    pdfCanvas.AddXObject(watermarkXObject, 0, -scale, scale, 0,
                        (documentPageSize.GetWidth() - backgroundHeight) / 2,
                        backgroundWidth + (documentPageSize.GetHeight() - backgroundWidth) / 2);
                    break;

                case 180:
                    scaleWidth = documentPageSize.GetWidth() / watermarkPageSize.GetWidth();
                    scaleHeight = documentPageSize.GetHeight() / watermarkPageSize.GetHeight();
                    scale = scaleWidth < scaleHeight ? scaleWidth : scaleHeight;
                    scale = fitToPage ? scale : 1;

                    backgroundHeight = scale * watermarkPageSize.GetHeight();
                    backgroundWidth = scale * watermarkPageSize.GetWidth();

                    pdfCanvas.AddXObject(watermarkXObject, -scale, 0, 0, -scale,
                        backgroundWidth + (documentPageSize.GetWidth() - backgroundWidth) / 2,
                        backgroundHeight + (documentPageSize.GetHeight() - backgroundHeight) / 2);
                    break;

                case 270:
                    scaleWidth = documentPageSize.GetWidth() / watermarkPageSize.GetHeight();
                    scaleHeight = documentPageSize.GetHeight() / watermarkPageSize.GetWidth();
                    scale = scaleWidth < scaleHeight ? scaleWidth : scaleHeight;
                    scale = fitToPage ? scale : 1;

                    backgroundHeight = scale * watermarkPageSize.GetHeight();
                    backgroundWidth = scale * watermarkPageSize.GetWidth();

                    pdfCanvas.AddXObject(watermarkXObject, 0, scale, -scale, 0,
                        backgroundHeight + (documentPageSize.GetWidth() - backgroundHeight) / 2,
                        (documentPageSize.GetHeight() - backgroundWidth) / 2);
                    break;

                case 0:
                default:
                    scaleWidth = documentPageSize.GetWidth() / watermarkPageSize.GetWidth();
                    scaleHeight = documentPageSize.GetHeight() / watermarkPageSize.GetHeight();
                    scale = scaleWidth < scaleHeight ? scaleWidth : scaleHeight;
                    scale = fitToPage ? scale : 1;

                    backgroundHeight = scale * watermarkPageSize.GetHeight();
                    backgroundWidth = scale * watermarkPageSize.GetWidth();

                    pdfCanvas.AddXObject(watermarkXObject, scale, 0, 0, scale, (documentPageSize.GetWidth() - backgroundWidth) / 2,
                        (documentPageSize.GetHeight() - backgroundHeight) / 2);
                    break;
            }
        }

        // -- clockwise --              cos  sin  -sin  cos  dx  dy
        //background.AddTemplate(page,  1f,   0,    0,  1f,  0,  0 ); //0°
        //background.AddTemplate(page,  0,  -1f,   1f,   0,  0,  0 ); //90°
        //background.AddTemplate(page, -1f,   0,    0, -1f,  0,  0 ); //180°
        //background.AddTemplate(page,  0,   1f,  -1f,   0,  0,  0 ); //270°
    }
}
