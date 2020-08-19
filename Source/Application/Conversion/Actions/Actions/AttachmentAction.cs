using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class AttachmentAction : IConversionAction, ICheckable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public AttachmentAction(IFile file, IPathUtil pathUtil)
        {
            _file = file;
            _pathUtil = pathUtil;
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            processor.AddAttachment(job);
        }

        public ActionResult ProcessJob(Job job)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.AttachmentPage.Enabled;
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.AttachmentPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.AttachmentPage.File);
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            if (!profile.AttachmentPage.Enabled)
                return new ActionResult();

            var isJobLevelCheck = checkLevel == CheckLevel.Job;

            if (string.IsNullOrEmpty(profile.AttachmentPage.File))
            {
                _logger.Error("No attachment file is specified.");
                return new ActionResult(ErrorCode.Attachment_NoFileSpecified);
            }

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(profile.AttachmentPage.File))
                return new ActionResult();

            if (!profile.AttachmentPage.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error("The attachment file \"" + profile.AttachmentPage.File + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Attachment_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.AttachmentPage.File);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.Attachment_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.Attachment_PathTooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.Attachment_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.Attachment_IllegalCharacters);
            }

            if (!isJobLevelCheck && profile.AttachmentPage.File.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(profile.AttachmentPage.File))
            {
                _logger.Error("The attachment file \"" + profile.AttachmentPage.File + "\" does not exist.");
                return new ActionResult(ErrorCode.Attachment_FileDoesNotExist);
            }

            return new ActionResult();
        }
    }
}
