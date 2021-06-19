using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class BackgroundAction : ActionBase<BackgroundPage>, IConversionAction
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public BackgroundAction(IFile file, IPathUtil pathUtil)
            : base(p => p.BackgroundPage)
        {
            _file = file;
            _pathUtil = pathUtil;
        }

        protected override ActionResult DoProcessJob(Job job)
        {
            throw new System.NotImplementedException();
        }

        public void ProcessJob(IPdfProcessor pdfProcessor, Job job)
        {
            pdfProcessor.AddBackground(job);
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.BackgroundPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.BackgroundPage.File);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!profile.BackgroundPage.Enabled)
                return new ActionResult();

            if (string.IsNullOrEmpty(profile.BackgroundPage.File))
            {
                _logger.Error("No background file is specified.");
                return new ActionResult(ErrorCode.Background_NoFileSpecified);
            }

            var isJobLevelCheck = checkLevel == CheckLevel.RunningJob;

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(profile.BackgroundPage.File))
                return new ActionResult();

            if (!profile.BackgroundPage.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error("The background file \"" + profile.BackgroundPage.File + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Background_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.BackgroundPage.File);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.Background_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.Background_PathTooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.Background_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.Background_IllegalCharacters);
            }

            if (!isJobLevelCheck && profile.BackgroundPage.File.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(profile.BackgroundPage.File))
            {
                _logger.Error("The background file \"" + profile.BackgroundPage.File + "\" does not exist.");
                return new ActionResult(ErrorCode.Background_FileDoesNotExist);
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
