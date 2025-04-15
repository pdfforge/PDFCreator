using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextPageNumbersAdder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly FontPathHelper _fontPathHelper;
        private readonly PageNumbersHelper _pageNumbersHelper;

        public ITextPageNumbersAdder(IFile file)
        {
            _fontPathHelper = new FontPathHelper(file);
            _pageNumbersHelper = new PageNumbersHelper();
        }

        internal void AddPageNumbers(PdfDocument pdfDocument, ConversionProfile profile)
        {
            try
            {
                _logger.Debug("Start adding page numbers.");
                var settings = profile.PageNumbers;
                var beginOn = settings.BeginOn;
                var beginWith = settings.BeginWith;
                var format = settings.Format;
                var fontFile = settings.FontFile;
                //var fontName = settings.FontName; //Unused in here
                var fontColor = settings.FontColor;
                var fontSize = settings.FontSize;
                var horizontalOffset = settings.HorizontalOffset;
                var verticalOffset = settings.VerticalOffset;
                var useRoman = settings.UseRomanNumerals;
                var pageAlignment = settings.Position;
                var alternateCorner = settings.AlternateCorner;

                Document doc = new Document(pdfDocument);

                var result = _fontPathHelper.TryGetFontPath(fontFile, out var fontPath);
                if (!result)
                    throw new ProcessingException("Error during font path detection.", ErrorCode.Stamp_FontNotFound);
                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, true);
                doc.SetFont(font);

                var color = new DeviceRgb(fontColor.R, fontColor.G, fontColor.B);
                doc.SetFontColor(color);
                doc.SetFontSize(fontSize);

                var numberOfPages = pdfDocument.GetNumberOfPages();
                var pageNumberToWrite = beginWith;

                if (beginWith > numberOfPages)
                    return; //Do not call doc.Flush if no page numbers get added

                for (var pageNumberInDocument = beginOn; pageNumberInDocument <= numberOfPages; pageNumberInDocument++)
                {
                    var page = pdfDocument.GetPage(pageNumberInDocument);
                    var size = page.GetPageSize();
                    var text = _pageNumbersHelper.FormatPageNumber(format, pageNumberToWrite, numberOfPages, useRoman);
                    var currentAlignment = pageAlignment;
                    if (alternateCorner && pageNumberInDocument % 2 == 0)
                    {
                        currentAlignment = _pageNumbersHelper.Alternate(currentAlignment);
                    }
                    var (posX, posY) = _pageNumbersHelper.CalculateUserOffset(horizontalOffset, verticalOffset, size.GetHeight(), size.GetWidth(), currentAlignment);
                    var (horizontalAlignment, verticalAlignment) = GetAlignment(currentAlignment);

                    doc.ShowTextAligned(new Paragraph(text), posX, posY, pageNumberInDocument, horizontalAlignment, verticalAlignment, 0);
                    pageNumberToWrite++;
                }

                doc.Flush();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType() + " while adding page numbers.";

                throw new ProcessingException(errorMessage, ErrorCode.PageNumbers_GenericError, ex);
            }
        }

        private Tuple<TextAlignment, VerticalAlignment> GetAlignment(PageNumberPosition pageAlignment)
        {
            VerticalAlignment verticalAlignment;
            switch (pageAlignment)
            {
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.BottomCenter:
                    verticalAlignment = VerticalAlignment.BOTTOM;
                    break;

                case PageNumberPosition.TopRight:
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.TopCenter:
                    verticalAlignment = VerticalAlignment.TOP;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pageAlignment), pageAlignment, null);
            }

            TextAlignment horizontalAlignment;
            switch (pageAlignment)
            {
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.BottomLeft:
                    horizontalAlignment = TextAlignment.LEFT;
                    break;

                case PageNumberPosition.TopRight:
                case PageNumberPosition.BottomRight:
                    horizontalAlignment = TextAlignment.RIGHT;
                    break;

                case PageNumberPosition.TopCenter:
                case PageNumberPosition.BottomCenter:
                    horizontalAlignment = TextAlignment.CENTER;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pageAlignment), pageAlignment, null);
            }

            return new Tuple<TextAlignment, VerticalAlignment>(horizontalAlignment, verticalAlignment);
        }
    }
}
