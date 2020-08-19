using iText.Kernel.Pdf;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextPdfMerger
    {
        private enum MergeMode
        {
            Prepend,
            Append
        }

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public void AddCover(PdfDocument document, params string[] cover)
        {
            try
            {
                _logger.Debug("Start adding cover.");
                DoMergePdfs(document, cover, MergeMode.Prepend);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType() + " while adding cover.";
                throw new ProcessingException(errorMessage, ErrorCode.Cover_GenericError, ex);
            }
        }

        public void AddAttachment(PdfDocument document, params string[] attachment)
        {
            try
            {
                _logger.Debug("Start adding attachment.");
                DoMergePdfs(document, attachment, MergeMode.Append);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType() + " while adding attachment.";
                throw new ProcessingException(errorMessage, ErrorCode.Attachment_GenericError, ex);
            }
        }

        private void DoMergePdfs(PdfDocument document, IList<string> pdfLocations, MergeMode position)
        {
            if (pdfLocations.Count <= 0)
            {
                return;
            }

            foreach (var file in pdfLocations)
            {
                _logger.Trace("Merge document with: " + file);
                PdfReader reader = new PdfReader(file);
                var doc = new PdfDocument(reader);

                var startIndex = position == MergeMode.Prepend ? 1 : document.GetNumberOfPages() + 1;

                doc.CopyPagesTo(Enumerable.Range(1, doc.GetNumberOfPages()).ToList(), document, startIndex);

                reader.Close();
                doc.Close();
            }
        }
    }
}
