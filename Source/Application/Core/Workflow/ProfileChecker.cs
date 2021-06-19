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
        ActionResultDict CheckProfileList(CurrentCheckSettings settings);

        ActionResult CheckFileNameAndTargetDirectory(ConversionProfile profile);

        ActionResult CheckProfile(ConversionProfile profile, CurrentCheckSettings settings);

        ActionResult CheckJob(Job job);
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

        private ActionResult CheckFileNameAndTargetDirectory(ConversionProfile profile, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            actionResult.AddRange(CheckTargetDirectory(profile, checkLevel));
            actionResult.AddRange(CheckFileNameTemplate(profile, checkLevel));

            return actionResult;
        }

        private ActionResult ProfileCheck(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var actionResult = CheckFileNameAndTargetDirectory(profile, checkLevel);

            foreach (var action in _actions)
            {
                if (!action.IsEnabled(profile))
                    continue;

                if (checkLevel == CheckLevel.RunningJob)
                    if (action.IsRestricted(profile))
                        continue;

                var result = action.Check(profile, settings, checkLevel);
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
            if (checkLevel == CheckLevel.RunningJob)
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
            if (checkLevel == CheckLevel.RunningJob)
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

        public ActionResult CheckFileNameAndTargetDirectory(ConversionProfile profile)
        {
            return CheckFileNameAndTargetDirectory(profile, CheckLevel.EditingProfile);
        }

        public ActionResult CheckProfile(ConversionProfile profile, CurrentCheckSettings settings)
        {
            return ProfileCheck(profile, settings, CheckLevel.EditingProfile);
        }

        public ActionResultDict CheckProfileList(CurrentCheckSettings settings)
        {
            var nameResultDict = new ActionResultDict();

            foreach (var profile in settings.Profiles)
            {
                var result = ProfileCheck(profile, settings, CheckLevel.EditingProfile);
                if (!result)
                    nameResultDict.Add(profile.Name, result);
            }

            return nameResultDict;
        }

        public ActionResult CheckJob(Job job)
        {
            job.Profile.FileNameTemplate = job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate);

            foreach (var action in _actions)
            {
                if (action.IsEnabled(job.Profile) && !action.IsRestricted(job.Profile))
                    action.ApplyPreSpecifiedTokens(job);
            }

            var actionResult = job.Profile.SaveFileTemporary ? new ActionResult() : CheckJobOutputFilenameTemplate(job.OutputFileTemplate);

            var settings = new CurrentCheckSettings(job.AvailableProfiles, job.PrinterMappings, job.Accounts);
            actionResult.AddRange(ProfileCheck(job.Profile, settings, CheckLevel.RunningJob));
            return actionResult;
        }
    }
}
