using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class WatermarkAction : ActionBase<Watermark>, IConversionAction
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public WatermarkAction(IFile file, IPathUtil pathUtil)
            : base(p => p.Watermark)
        {
            _file = file;
            _pathUtil = pathUtil;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            processor.AddWatermark(job);
            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Watermark.File = job.TokenReplacer.ReplaceTokens(job.Profile.Watermark.File);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!profile.Watermark.Enabled)
                return new ActionResult();

            if (string.IsNullOrEmpty(profile.Watermark.File))
            {
                _logger.Error("No watermark file is specified.");
                return new ActionResult(ErrorCode.Watermark_NoFileSpecified);
            }

            var isJobLevelCheck = checkLevel == CheckLevel.RunningJob;

            if (!isJobLevelCheck && !profile.UserTokens.Enabled && TokenIdentifier.ContainsUserToken(profile.Watermark.File))
                return new ActionResult(ErrorCode.Watermark_RequiresUserTokens);

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
                case PathUtilStatus.InvalidPath:
                    return new ActionResult(ErrorCode.Watermark_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.Watermark_PathTooLong);
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

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
