using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.XMP;
using iText.Pdfa;
using iText.Signatures;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.IO;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextPdfProcessor : PdfProcessorBase
    {
        private readonly ITextStampAdder _stampAdder;
        private string _originalSourceFile;
        private PdfDocument _pdfDocument;

        // StampAdder needs to be created via container
        public ITextPdfProcessor(IFile file, ITextStampAdder stampAdder) : base(file)
        {
            _stampAdder = stampAdder;
        }

        private void EnsurePdfDocumentInit(Job job)
        {
            if (_pdfDocument == null)
                InitPdfDocument(job);
        }

        /// <summary>
        ///     Determines number of pages in PDF file
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <returns>Number of pages in pdf file</returns>
        public override int GetNumberOfPages(string pdfFile)
        {
            using (var pdfReader = new PdfReader(pdfFile))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                var numberOfPages = pdfDocument.GetNumberOfPages();
                return numberOfPages;
            }
        }

        private string AddTailToFile(string file, string tail)
        {
            if (string.IsNullOrEmpty(file))
                return "";

            var newFileName = Path.GetFileNameWithoutExtension(file) + tail + Path.GetExtension(file);
            var newFile = Path.Combine(Path.GetDirectoryName(file), newFileName);
            return newFile;
        }

        public override void AddAttachment(Job job)
        {
            if (!job.Profile.AttachmentPage.Enabled)
                return;

            EnsurePdfDocumentInit(job);

            var pdfMerger = new ITextPdfMerger();
            pdfMerger.AddAttachment(_pdfDocument, job.Profile.AttachmentPage.File);
        }

        public override void AddCover(Job job)
        {
            if (!job.Profile.CoverPage.Enabled)
                return;

            EnsurePdfDocumentInit(job);

            var pdfMerger = new ITextPdfMerger();
            pdfMerger.AddCover(_pdfDocument, job.Profile.CoverPage.File);
        }

        public override void AddStamp(Job job)
        {
            if (!job.Profile.Stamping.Enabled)
                return;

            EnsurePdfDocumentInit(job);

            _stampAdder.AddStamp(_pdfDocument, job.Profile);
        }

        public override void AddBackground(Job job)
        {
            if (!job.Profile.BackgroundPage.Enabled)
                return;

            EnsurePdfDocumentInit(job);

            var watermarkAdder = new ITextWatermarkAdder();
            watermarkAdder.AddBackground(_pdfDocument, job.Profile);
        }

        public override void AddWatermark(Job job)
        {
            if (!job.Profile.Watermark.Enabled)
                return;

            EnsurePdfDocumentInit(job);

            var watermarkAdder = new ITextWatermarkAdder();
            watermarkAdder.AddForeground(_pdfDocument, job.Profile);
        }

        private PdfVersion DeterminePdfVersionAsEnum(ConversionProfile profile)
        {
            var intendedPdfVersion = DeterminePdfVersion(profile);
            if (intendedPdfVersion == "1.7")
                return PdfVersion.PDF_1_7;
            if (intendedPdfVersion == "1.6")
                return PdfVersion.PDF_1_6;
            if (intendedPdfVersion == "1.5")
                return PdfVersion.PDF_1_5;
            if (intendedPdfVersion == "1.4")
                return PdfVersion.PDF_1_4;

            return PdfVersion.PDF_1_3;
        }

        private PdfDocument InitPdfDocument(Job job)
        {
            var version = DeterminePdfVersionAsEnum(job.Profile);

            _originalSourceFile = job.IntermediatePdfFile;
            var processedFile = AddTailToFile(_originalSourceFile, "_processed");

            var writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(version);

            ApplyEncryptionToWriterProperties(job, writerProperties);

            try
            {
                PdfReader pdfReader = new PdfReader(_originalSourceFile);
                PdfWriter pdfWriter = new PdfWriter(processedFile, writerProperties);

                _pdfDocument = new PdfDocument(pdfReader, pdfWriter);
            }
            catch (Exception e)
            {
                Logger.Trace(e.Message);
            }

            job.IntermediatePdfFile = processedFile;

            return _pdfDocument;
        }

        protected override void DoSignEncryptAndConvertPdfAAndWritePdf(Job job)
        {
            EnsurePdfDocumentInit(job);
            UpdateMetadata(_pdfDocument, job);
            SetStartPage(_pdfDocument, job);
            WritePdfDocument(job);
            ConvertToPdfa(job);
            SignPdf(job);
        }

        private void WritePdfDocument(Job job)
        {
            try
            {
                _pdfDocument.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while closing pdfDocument ");
                throw;
            }
            finally
            {
                File.Delete(_originalSourceFile);
            }
        }

        private static void SetStartPage(PdfDocument pdfDocument, Job job)
        {
            var pageIndex = job.Profile.PdfSettings.ViewerStartsOnPage;
            // index gets limited to at least 1 and the number of documents
            pageIndex = Math.Max(1, Math.Min(pageIndex, pdfDocument.GetNumberOfPages()));

            var navigationPage = pdfDocument.GetPage(pageIndex);
            pdfDocument.GetCatalog().SetOpenAction(PdfExplicitDestination.CreateXYZ(navigationPage, 0, 0, 1));
        }

        private void ConvertToPdfa(Job job)
        {
            if (job.Profile.OutputFormat.IsPdfA())
            {
                var sourcePdf = job.IntermediatePdfFile;

                if (string.IsNullOrEmpty(sourcePdf))
                    return;

                var targetPdf = AddTailToFile(sourcePdf, "_pdfa");

                var version = DeterminePdfVersionAsEnum(job.Profile);
                var writerProperties = new WriterProperties();
                writerProperties.SetPdfVersion(version);

                byte[] resource;
                //Set ICC Profile according to the color model
                switch (job.Profile.PdfSettings.ColorModel)
                {
                    case ColorModel.Cmyk:
                        resource = GhostscriptResources.WebCoatedFOGRA28;
                        break;

                    case ColorModel.Gray:
                        resource = GhostscriptResources.ISOcoated_v2_grey1c_bas;
                        break;

                    default:
                        resource = GhostscriptResources.eciRGB_v2;
                        break;
                }

                try
                {
                    using (var icc = new MemoryStream(resource))
                    {
                        var document = new PdfADocument(new PdfWriter(targetPdf, writerProperties), GetConformLevel(job.Profile.OutputFormat), new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", icc));

                        document.SetTagged();
                        var toCopyDoc = new PdfDocument(new PdfReader(sourcePdf));
                        toCopyDoc.CopyPagesTo(1, toCopyDoc.GetNumberOfPages(), document);
                        var metaData = XMPMetaFactory.ParseFromBuffer(toCopyDoc.GetXmpMetadata());
                        document.SetXmpMetadata(metaData);
                        toCopyDoc.Close();
                        document.Close();

                        File.Delete(sourcePdf);
                        job.IntermediatePdfFile = targetPdf;
                    }
                }
                catch (PdfAConformanceException ex)
                {
                    throw new ProcessingException("One of the used pdfs does not conform to the PDF-A specification", ErrorCode.Processing_ConformanceMismatch, ex);
                }
            }
        }

        private PdfAConformanceLevel GetConformLevel(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.PdfA1B:
                    return PdfAConformanceLevel.PDF_A_1B;

                case OutputFormat.PdfA2B:
                    return PdfAConformanceLevel.PDF_A_2B;

                case OutputFormat.PdfA3B:
                    return PdfAConformanceLevel.PDF_A_3B;

                default:
                    return null;
            }
        }

        private void SignPdf(Job job)
        {
            if (!job.Profile.PdfSettings.Signature.Enabled)
                return;

            ReaderProperties readerProperties = new ReaderProperties();

            if (job.Profile.PdfSettings.Security.Enabled)
                readerProperties.SetPassword(Encoding.Default.GetBytes(job.Passwords.PdfOwnerPassword));

            var sourceFile = job.IntermediatePdfFile;
            var targetFile = AddTailToFile(sourceFile, "_processed");

            using (PdfReader pdfReader = new PdfReader(sourceFile, readerProperties))
            using (PdfWriter pdfWriter = new PdfWriter(targetFile))
            {
                var signer = new PdfSigner(pdfReader, pdfWriter, new StampingProperties().PreserveEncryption().UseAppendMode());
                using (signer.GetDocument())
                {
                    try
                    {
                        new ITextSigner().SignPdfFile(signer, job.Profile, job.Passwords, job.Accounts);
                    }
                    catch (ProcessingException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Could not close iText pdf stamper.\r\n{ex}");
                    }
                }
            }

            job.IntermediatePdfFile = targetFile;
            File.Delete(sourceFile);
        }

        private void ApplyEncryptionToWriterProperties(Job job, WriterProperties writerProperties)
        {
            if (!job.Profile.PdfSettings.Security.Enabled || job.Profile.OutputFormat.IsPdfA())
                return;

            new Encrypter().SetEncryption(writerProperties, job.Profile, job.Passwords);
        }

        private void UpdateMetadata(PdfDocument document, Job job)
        {
            if (!job.Profile.OutputFormat.IsPdfA())
                return;

            var xmpMetadataUpdater = new XmpMetadataUpdater();
            xmpMetadataUpdater.UpdateXmpMetadata(document, job.Profile, job);
        }
    }
}
