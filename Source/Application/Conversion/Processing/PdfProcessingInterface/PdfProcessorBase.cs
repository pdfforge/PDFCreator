using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public abstract class PdfProcessorBase : IPdfProcessor
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly IFile File;

        protected PdfProcessorBase(IFile file)
        {
            File = file;
        }

        /// <summary>
        ///     Process PDF with updating metadata (for PDF/A), Encryption, adding background and signing according to profile.
        /// </summary>
        public void SignEncryptConvertPdfAAndWriteFile(Job job)
        {
            var pdfFile = job.IntermediatePdfFile;
            Logger.Debug("Start processing of " + pdfFile);

            if (!File.Exists(pdfFile))
            {
                throw new ProcessingException("_file in PdfProcessor does not exist: " + pdfFile, ErrorCode.Processing_OutputFileMissing);
            }

            const int retryCount = 5;
            var retryInterval = TimeSpan.FromMilliseconds(300);

            try
            {
                // Retry signing when a ProcessingException with the ErrorCode Signature_NoTimeServerConnection was thrown
                Retry.Do(
                        () => DoSignEncryptAndConvertPdfAAndWritePdf(job),
                        retryInterval: retryInterval,
                        retryCount: retryCount,
                        retryCondition: ex => (ex as ProcessingException)?.ErrorCode == ErrorCode.Signature_NoTimeServerConnection);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions.First();
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.GetType() + " while processing file:" + pdfFile);
                throw new ProcessingException(
                    ex.GetType() + " while processing file:" + pdfFile + Environment.NewLine + ex.Message, ErrorCode.Processing_GenericError, ex);
            }
        }

        /// <summary>
        ///     Determines number of pages in PDF file
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <returns>Number of pages in pdf file</returns>
        public abstract int GetNumberOfPages(string pdfFile);

        /// <summary>
        ///     Determine PDF-Version according to settings in conversion profile.
        /// </summary>
        /// <param name="profile">ConversionProfile</param>
        /// <returns>PDF Version as string as interim value for inherited Processors</returns>
        public string DeterminePdfVersion(ConversionProfile profile)
        {
            //Important:
            //The string value is just an interim value!!!
            //If another PDFVersion is added, the inherited Processors have to evaluate it

            var pdfVersion = "1.3";

            if (profile.OutputFormat != OutputFormat.PdfX)
                pdfVersion = "1.4";

            if (profile.PdfSettings.Signature.Enabled)
                pdfVersion = "1.4"; //todo: Could remain 1.3. Is it necessary to fix this?

            if (profile.BackgroundPage.Enabled)
                pdfVersion = "1.4"; //todo: Could remain 1.3. Is it necessary to fix this?

            if (profile.OutputFormat == OutputFormat.Pdf)
                if (profile.PdfSettings.Security.Enabled)
                    if (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit)
                        pdfVersion = "1.6";

            if (profile.OutputFormat != OutputFormat.PdfA1B)
                if (profile.PdfSettings.Signature.Enabled)
                    if (!profile.PdfSettings.Signature.AllowMultiSigning)
                        pdfVersion = "1.6";

            if (profile.OutputFormat == OutputFormat.Pdf)
                if (profile.PdfSettings.Security.Enabled)
                    if (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes256Bit)
                        pdfVersion = "1.7";

            if (profile.OutputFormat == OutputFormat.PdfA2B || profile.OutputFormat == OutputFormat.PdfA3B)
                pdfVersion = "1.7";

            return pdfVersion;
        }

        public abstract void AddAttachment(Job job);

        public abstract void AddCover(Job job);

        public abstract void AddStamp(Job job);

        public abstract void AddBackground(Job job);

        public abstract void AddWatermark(Job job);

        protected abstract void DoSignEncryptAndConvertPdfAAndWritePdf(Job job);
    }
}
