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
        /// Inits the Profile
        /// </summary>
        /// <param name="job"></param>
        public void Init(Job job)
        {
            Logger.Trace("Init Processor");

            //Must be applied before determining passwords
            ApplyRestrictionsToProfile(job);
        }

        private void ApplyRestrictionsToProfile(Job job)
        {
            switch (job.Profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                    if (job.Profile.PdfSettings.Security.Enabled)
                    {
                        job.Profile.PdfSettings.Security.Enabled = false;
                        Logger.Warn("Encryption automatically disabled for PDF/A1-b");
                    }
                    if (job.Profile.PdfSettings.Signature.Enabled)
                    {
                        if (!job.Profile.PdfSettings.Signature.AllowMultiSigning)
                        {
                            job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
                            Logger.Warn("Allow multiple signing automatically enabled for PDF/A1-b");
                        }
                    }
                    return;

                case OutputFormat.PdfX:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                    if (job.Profile.PdfSettings.Security.Enabled)
                    {
                        job.Profile.PdfSettings.Security.Enabled = false;
                        Logger.Warn("Encryption automatically disabled for" + job.Profile.OutputFormat);
                    }
                    return;
            }

            if (!job.Profile.PdfSettings.Security.AllowPrinting)
                job.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
        }

        /// <summary>
        ///     Check if processing is required.
        ///     Therefore the output format must be pdf and security or background page or signing must be enabled.
        ///     Furthermore PDF/A requires processing the metadata.
        /// </summary>
        /// <param name="profile">Profile with PDF settings</param>
        /// <returns>True if processing is required</returns>
        public bool ProcessingRequired(ConversionProfile profile)
        {
            switch (profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                    return true; //Always true because of the metadata update

                case OutputFormat.Pdf:
                    return profile.PdfSettings.Security.Enabled
                           || profile.BackgroundPage.Enabled
                           || profile.PdfSettings.Signature.Enabled;

                case OutputFormat.PdfX:
                    return profile.BackgroundPage.Enabled
                           || profile.PdfSettings.Signature.Enabled;
            }

            return false;
        }

        /// <summary>
        ///     Process PDF with updating metadata (for PDF/A), Encryption, adding background and signing according to profile.
        /// </summary>
        public void ProcessPdf(Job job)
        {
            var pdfFile = job.TempOutputFiles.First();
            Logger.Debug("Start processing of " + pdfFile);

            if (!File.Exists(pdfFile))
            {
                throw new ProcessingException("_file in PdfProcessor does not exist: " + pdfFile, ErrorCode.Processing_OutputFileMissing);
            }

            ApplyTokens(job);

            const int retryCount = 5;
            var retryInterval = TimeSpan.FromMilliseconds(300);

            try
            {
                // Retry signing when a ProcessingException with the ErrorCode Signature_NoTimeServerConnection was thrown
                Retry.Do(
                        () => DoProcessPdf(job),
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
                Logger.Error(ex.GetType() + " while processing file:" + pdfFile + Environment.NewLine + ex.Message);
                throw new ProcessingException(
                    ex.GetType() + " while processing file:" + pdfFile + Environment.NewLine + ex.Message, ErrorCode.Processing_GenericError);
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

        private void ApplyTokens(Job job)
        {
            try
            {
                if (job.Profile.PdfSettings.Signature.Enabled)
                {
                    job.Profile.PdfSettings.Signature.SignReason = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignReason);
                    job.Profile.PdfSettings.Signature.SignContact = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignContact);
                    job.Profile.PdfSettings.Signature.SignLocation = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignLocation);
                }

                if (job.Profile.BackgroundPage.Enabled)
                    job.Profile.BackgroundPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.BackgroundPage.File);
            }
            catch (Exception ex)
            {
                Logger.Warn("Exception while replacing tokens in signature metadata: " + ex.Message);
            }
        }

        protected abstract void DoProcessPdf(Job job);
    }
}
