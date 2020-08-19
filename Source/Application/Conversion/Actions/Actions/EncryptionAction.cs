using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class EncryptionAction : IConversionAction, ICheckable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public ActionResult ProcessJob(Job job)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            if (!profile.OutputFormat.IsPdf())
                return false;

            return profile.PdfSettings.Security.Enabled;
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            //nothing to do here. The Encryption must be triggered as last processing step in the ActionManager
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            //nothing to do here.
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            if (!IsEnabled(profile))
                return result;

            var security = profile.PdfSettings.Security;

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrEmpty(security.OwnerPassword))
                {
                    _logger.Error("No saved owner password for security in automatic saving.");
                    result.Add(ErrorCode.AutoSave_NoOwnerPassword);
                }

                if (security.RequireUserPassword)
                {
                    if (string.IsNullOrEmpty(security.UserPassword))
                    {
                        _logger.Error("No saved user password for security in automatic saving.");
                        result.Add(ErrorCode.AutoSave_NoUserPassword);
                    }
                }
            }

            if (checkLevel == CheckLevel.Profile)
                return result;

            return result;
        }
    }
}
