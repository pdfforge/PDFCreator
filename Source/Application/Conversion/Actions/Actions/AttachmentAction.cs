﻿using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class AttachmentAction : ActionBase<AttachmentPage>, IConversionAction
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public AttachmentAction(IFile file, IPathUtil pathUtil)
            : base(p => p.AttachmentPage)
        {
            _file = file;
            _pathUtil = pathUtil;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            processor.AddAttachment(job);
            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.AttachmentPage.Files = job.Profile.AttachmentPage.Files.Select(job.TokenReplacer.ReplaceTokens).ToList();
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!profile.AttachmentPage.Enabled)
                return new ActionResult();
            ActionResult totalResult = new ActionResult();
            foreach (var file in profile.AttachmentPage.Files.DefaultIfEmpty())
            {
                var result = CheckFile(file, checkLevel, profile.UserTokens.Enabled);
                foreach (var errorCode in result)
                    if (!totalResult.Contains(errorCode))
                        totalResult.Add(errorCode);
            }

            return totalResult;
        }

        private ActionResult CheckFile(string file, CheckLevel checkLevel, bool userTokenEnabled)
        {
            var isJobLevelCheck = checkLevel == CheckLevel.RunningJob;

            if (string.IsNullOrEmpty(file))
            {
                _logger.Error("No attachment file is specified.");
                return new ActionResult(ErrorCode.Attachment_NoFileSpecified);
            }

            if (!isJobLevelCheck && !userTokenEnabled && TokenIdentifier.ContainsUserToken(file))
                return new ActionResult(ErrorCode.Attachment_RequiresUserTokens);

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(file))
                return new ActionResult();

            if (!file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Error("The attachment file \"" + file + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Attachment_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(file);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidPath:
                    return new ActionResult(ErrorCode.Attachment_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.Attachment_PathTooLong);
            }

            if (!isJobLevelCheck && file.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(file))
            {
                _logger.Error("The attachment file \"" + file + "\" does not exist.");
                return new ActionResult(ErrorCode.Attachment_FileDoesNotExist);
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
