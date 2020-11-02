using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Linq;
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
            job.Profile.CoverPage.Files = job.Profile.CoverPage.Files.Select(job.TokenReplacer.ReplaceTokens).ToList();
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            if (!profile.CoverPage.Enabled)
                return new ActionResult();
            ActionResult totalResult = new ActionResult();
            foreach (var file in profile.CoverPage.Files.DefaultIfEmpty())
            {
                totalResult.Add(CheckFile(file, checkLevel));
            }

            return totalResult;
        }

        private ActionResult CheckFile(string file, CheckLevel checkLevel)
        {
            var isJobLevelCheck = checkLevel == CheckLevel.Job;

            if (string.IsNullOrEmpty(file))
            {
                _logger.Error("No cover file is specified.");
                return new ActionResult(ErrorCode.Cover_NoFileSpecified);
            }

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(file))
                return new ActionResult();

            if (!file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error("The cover file \"" + file + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Cover_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(file);
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

            if (!isJobLevelCheck && file.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(file))
            {
                _logger.Error("The cover file \"" + file + "\" does not exist.");
                return new ActionResult(ErrorCode.Cover_FileDoesNotExist);
            }

            return new ActionResult();
        }
    }
}
