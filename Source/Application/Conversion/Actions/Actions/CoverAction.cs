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
    public class CoverAction : IConversionAction, ICheckable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public CoverAction(IFile file, IPathUtil pathUtil)
        {
            _file = file;
            _pathUtil = pathUtil;
        }

        public ActionResult ProcessJob(Job job)
        {
            throw new System.NotImplementedException();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.CoverPage.Enabled;
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            processor.AddCover(job);
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.CoverPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.CoverPage.File);
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            if (!profile.CoverPage.Enabled)
                return new ActionResult();

            var isJobLevelCheck = checkLevel == CheckLevel.Job;

            if (string.IsNullOrEmpty(profile.CoverPage.File))
            {
                _logger.Error("No cover file is specified.");
                return new ActionResult(ErrorCode.Cover_NoFileSpecified);
            }

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(profile.CoverPage.File))
                return new ActionResult();

            if (!profile.CoverPage.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error("The cover file \"" + profile.CoverPage.File + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Cover_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.CoverPage.File);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.Cover_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.Cover_PathTooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.Cover_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.Cover_IllegalCharacters);
            }

            if (!isJobLevelCheck && profile.CoverPage.File.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(profile.CoverPage.File))
            {
                _logger.Error("The cover file \"" + profile.CoverPage.File + "\" does not exist.");
                return new ActionResult(ErrorCode.Cover_FileDoesNotExist);
            }

            return new ActionResult();
        }
    }
}
