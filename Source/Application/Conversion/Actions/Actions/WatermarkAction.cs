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
    public class WatermarkAction : IConversionAction, ICheckable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public WatermarkAction(IFile file, IPathUtil pathUtil)
        {
            _file = file;
            _pathUtil = pathUtil;
        }

        public ActionResult ProcessJob(Job job)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.Watermark.Enabled;
        }

        public void ProcessJob(IPdfProcessor pdfProcessor, Job job)
        {
            pdfProcessor.AddWatermark(job);
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Watermark.File = job.TokenReplacer.ReplaceTokens(job.Profile.Watermark.File);
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            if (!profile.Watermark.Enabled)
                return new ActionResult();

            if (string.IsNullOrEmpty(profile.Watermark.File))
            {
                _logger.Error("No watermark file is specified.");
                return new ActionResult(ErrorCode.Watermark_NoFileSpecified);
            }

            var isJobLevelCheck = checkLevel == CheckLevel.Job;

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(profile.Watermark.File))
                return new ActionResult();

            if (!profile.Watermark.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error("The watermark file \"" + profile.Watermark.File + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Watermark_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.Watermark.File);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.Watermark_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.Watermark_PathTooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.Watermark_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.Watermark_IllegalCharacters);
            }

            if (!isJobLevelCheck && profile.Watermark.File.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(profile.Watermark.File))
            {
                _logger.Error("The watermark file \"" + profile.Watermark.File + "\" does not exist.");
                return new ActionResult(ErrorCode.Watermark_FileDoesNotExist);
            }

            return new ActionResult();
        }
    }
}
