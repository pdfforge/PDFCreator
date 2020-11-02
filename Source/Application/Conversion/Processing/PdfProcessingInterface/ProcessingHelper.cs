using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public interface IPdfProcessingHelper
    {
        void ApplyFormatRestrictionsToProfile(Job job);

        bool IsProcessingRequired(Job job);
    }

    public class PdfProcessingHelper : IPdfProcessingHelper
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IEnumerable<IAction> _actions;

        public PdfProcessingHelper(IEnumerable<IAction> actions)
        {
            _actions = actions;
        }

        /// <summary>
        /// Must be applied before determining passwords
        /// </summary>
        public void ApplyFormatRestrictionsToProfile(Job job)
        {
            if (!job.Profile.PdfSettings.Security.AllowPrinting)
                job.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;

            switch (job.Profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                    EnsureDisabledSecurity(job);
                    EnsureEnabledMultiSigning(job);
                    return;

                case OutputFormat.PdfX:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                    EnsureDisabledSecurity(job);
                    return;

                case OutputFormat.Jpeg:
                case OutputFormat.Png:
                case OutputFormat.Tif:
                case OutputFormat.Txt:
                    EnsureDisabledSecurity(job);
                    EnsureDisabledSigning(job);
                    return;

                case OutputFormat.Pdf:
                    return;

                default:
                    throw new NotSupportedException($"Please consider {job.Profile.OutputFormat} in PdfProcessingHelper.ApplyFormatRestrictionsToProfile");
            }
        }

        private void EnsureDisabledSecurity(Job job)
        {
            if (job.Profile.PdfSettings.Security.Enabled)
            {
                job.Profile.PdfSettings.Security.Enabled = false;
                Logger.Debug("Encryption disabled for " + job.Profile.OutputFormat);
            }
        }

        private void EnsureEnabledMultiSigning(Job job)
        {
            if (job.Profile.PdfSettings.Signature.Enabled)
            {
                if (!job.Profile.PdfSettings.Signature.AllowMultiSigning)
                {
                    job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
                    Logger.Debug("Allow multiple signing enabled for PDF/A1-b");
                }
            }
        }

        private void EnsureDisabledSigning(Job job)
        {
            if (job.Profile.PdfSettings.Signature.Enabled)
            {
                job.Profile.PdfSettings.Signature.Enabled = false;
                Logger.Debug("Signing disabled for " + job.Profile.OutputFormat);
            }
        }

        public bool IsProcessingRequired(Job job)
        {
            var profile = job.Profile;

            ApplyFormatRestrictionsToProfile(job);
            if (profile.OutputFormat.IsPdfA())
                return true; //Always true because of the metadata update

            return ConversionActionsEnabled(profile);
        }

        private bool ConversionActionsEnabled(ConversionProfile profile)
        {
            foreach (var action in _actions)
            {
                if (action is IConversionAction conversionAction)
                    if (conversionAction.IsEnabled(profile))
                        return true;
            }

            return false;
        }
    }
}
