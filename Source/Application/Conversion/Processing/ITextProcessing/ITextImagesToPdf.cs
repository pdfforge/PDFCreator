using iText.IO.Image;
using iText.Kernel.Pdf;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface.ImagesToPdf;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Collections.Generic;
using Document = iText.Layout.Document;
using Image = iText.Layout.Element.Image;
using PageSize = pdfforge.PDFCreator.Conversion.Settings.Enums.PageSize;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextImagesToPdf : IImagesToPdf
    {
        public void ConvertImage2Pdf(IList<string> directConversionFiles, ApplicationSettings applicationSettings, string outputFile)
        {
            var pageSize = SetPageSize(applicationSettings);

            var pdfDocument = new PdfDocument(new PdfWriter(outputFile));
            using var document = new Document(pdfDocument, pageSize);
            foreach (var file in directConversionFiles)
            {
                var imageData = ImageDataFactory.Create(file);
                var image = new Image(imageData);

                if (applicationSettings.PageSize == PageSize.Automatic)
                {
                    var width = image.GetImageScaledWidth();
                    var height = image.GetImageScaledHeight();
                    pageSize.SetWidth(width);
                    pageSize.SetHeight(height);
                }
                image.ScaleToFit(pageSize.GetWidth(), pageSize.GetHeight());
                var x = (pageSize.GetWidth() - image.GetImageScaledWidth()) / 2;
                var y = (pageSize.GetHeight() - image.GetImageScaledHeight()) / 2;

                document.SetMargins(y, x, y, x);

                document.Add(image);
            }
            if (applicationSettings.PageSize == PageSize.Automatic)
            {
                pdfDocument.SetDefaultPageSize(pageSize);
            }
            document.Close();
            pdfDocument.Close(); ;
        }

        private static iText.Kernel.Geom.PageSize SetPageSize(ApplicationSettings applicationSettings)
        {
            switch (applicationSettings.PageSize)
            {
                case PageSize.A4:
                    {
                        var size = iText.Kernel.Geom.PageSize.A4;
                        if (applicationSettings.PageOrientation == PageOrientation.Landscape)
                            size = size.Rotate();
                        return size;
                    }
                case PageSize.Legal:
                    {
                        var size = iText.Kernel.Geom.PageSize.LEGAL;
                        if (applicationSettings.PageOrientation == PageOrientation.Landscape)
                            size = size.Rotate();

                        return size;
                    }
                case PageSize.Letter:
                    {
                        var size = iText.Kernel.Geom.PageSize.LETTER;
                        if (applicationSettings.PageOrientation == PageOrientation.Landscape)
                            size = size.Rotate();
                        return size;
                    }
            }
            return applicationSettings.PageOrientation == PageOrientation.Landscape ? iText.Kernel.Geom.PageSize.Default.Rotate() : iText.Kernel.Geom.PageSize.Default;
        }
    }
}
