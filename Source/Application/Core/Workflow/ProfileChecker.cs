using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IProfileChecker
    {
        ActionResultDict CheckProfileList(IList<ConversionProfile> profileList, Accounts accounts);

        ActionResult CheckJob(Job job);

        ActionResult ProfileCheck(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel);
    }

    public class ProfileChecker : IProfileChecker
    {
        private readonly IEnumerable<IAction> _actions;
        private readonly IPathUtil _pathUtil;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProfileChecker(IPathUtil pathUtil, IEnumerable<IAction> actions)
        {
            _pathUtil = pathUtil;
            _actions = actions;
        }

        public ActionResult CheckJob(Job job)
        {
            job.Profile.FileNameTemplate = job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate);

            foreach (var action in _actions)
            {
                if (action is ICheckable checkable)
                    checkable.ApplyPreSpecifiedTokens(job);
            }

            var actionResult = job.Profile.SaveFileTemporary ? new ActionResult() : CheckJobOutputFilenameTemplate(job.OutputFileTemplate);

            actionResult.AddRange(ProfileCheck(job.Profile, job.Accounts, CheckLevel.Job));
            return actionResult;
        }

        public ActionResultDict CheckProfileList(IList<ConversionProfile> profileList, Accounts accounts)
        {
            var nameResultDict = new ActionResultDict();

            foreach (var profile in profileList)
            {
                var result = ProfileCheck(profile, accounts, CheckLevel.Profile);
                if (!result)
                    nameResultDict.Add(profile.Name, result);
            }

            return nameResultDict;
        }

        public ActionResult ProfileCheck(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            actionResult.AddRange(CheckTargetDirectory(profile, checkLevel));
            actionResult.AddRange(CheckFileNameTemplate(profile, checkLevel));

            foreach (var action in _actions)
            {
                if (!(action is ICheckable checkable))
                    continue;
                var result = checkable.Check(profile, accounts, checkLevel);
                actionResult.AddRange(result);
            }

            return actionResult;
        }

        private ActionResult CheckJobOutputFilenameTemplate(string outputFilenameTemplate)
        {
            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate);

            switch (pathUtilStatus)
            {
                case PathUtilStatus.PathWasNullOrEmpty:
                    return new ActionResult(ErrorCode.FilePath_InvalidRootedPath); //todo: Error Code no Template

                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.FilePath_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.FilePath_TooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.FilePath_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.FilePath_InvalidCharacters);

                case PathUtilStatus.Success:
                    break;
            }

            return new ActionResult();
        }

        private ActionResult CheckTargetDirectory(ConversionProfile profile, CheckLevel checkLevel)
        {
            if (checkLevel == CheckLevel.Job)
                return new ActionResult(); //Job uses Job.OutputFileTemplate

            if (profile.SaveFileTemporary)
                return new ActionResult();

            if (!profile.AutoSave.Enabled && string.IsNullOrWhiteSpace(profile.TargetDirectory))
                return new ActionResult(); // Valid LastSaveDirectory-Trigger

            if (profile.AutoSave.Enabled && string.IsNullOrWhiteSpace(profile.TargetDirectory))
                return new ActionResult(ErrorCode.TargetDirectory_NotSetForAutoSave);

            if (TokenIdentifier.ContainsTokens(profile.TargetDirectory))
                return new ActionResult();

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.TargetDirectory);

            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.TargetDirectory_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.TargetDirectory_TooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.TargetDirectory_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.TargetDirectory_IllegalCharacters);
            }

            return new ActionResult();
        }

        private ActionResult CheckFileNameTemplate(ConversionProfile profile, CheckLevel checkLevel)
        {
            if (checkLevel == CheckLevel.Job)
                return new ActionResult(); //Job uses Job.OutputFileTemplate

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrEmpty(profile.FileNameTemplate))
                {
                    _logger.Error("Automatic saving without filename template.");
                    return new ActionResult(ErrorCode.AutoSave_NoFilenameTemplate);
                }
            }

            if (TokenIdentifier.ContainsTokens(profile.FileNameTemplate))
                return new ActionResult();

            if (!_pathUtil.IsValidFilename(profile.FileNameTemplate))
                return new ActionResult(ErrorCode.FilenameTemplate_IllegalCharacters);

            return new ActionResult();
        }
    }
}
