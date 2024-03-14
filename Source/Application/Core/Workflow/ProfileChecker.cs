using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IProfileChecker
    {
        ActionResultDict CheckProfileList(CurrentCheckSettings settings);

        ActionResult CheckFileNameAndTargetDirectory(ConversionProfile profile);

        ActionResult CheckFileName(ConversionProfile profile);

        ActionResult CheckTargetDirectory(ConversionProfile profile);
        
        ActionResult CheckMetadata(ConversionProfile profile);

        ActionResult CheckProfile(ConversionProfile profile, CurrentCheckSettings settings);

        ActionResult CheckJob(Job job);

        bool DoesProfileContainRestrictedActions(ConversionProfile profile);
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
            actionResult.AddRange(CheckMetadata(profile, checkLevel));

            return actionResult;
        }

        public ActionResult CheckFileName(ConversionProfile profile)
        {
            return CheckFileNameTemplate(profile, CheckLevel.EditingProfile);
        }

        public ActionResult CheckTargetDirectory(ConversionProfile profile)
        {
            return CheckTargetDirectory(profile, CheckLevel.EditingProfile);
        }

        public ActionResult CheckMetadata(ConversionProfile profile)
        {
            return CheckMetadata(profile, CheckLevel.EditingProfile);
        }

        private ActionResult CheckMetadata(ConversionProfile profile, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            if (checkLevel == CheckLevel.EditingProfile && !profile.UserTokens.Enabled)
            {
                if(TokenIdentifier.ContainsUserToken(profile.TitleTemplate))
                    result.Add(ErrorCode.Metadata_Title_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.AuthorTemplate))
                    result.Add(ErrorCode.Metadata_Author_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.SubjectTemplate))
                    result.Add(ErrorCode.Metadata_Subject_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.KeywordTemplate))
                    result.Add(ErrorCode.Metadata_Keywords_RequiresUserToken);
            }

            return result;
        }

        public bool DoesProfileContainRestrictedActions(ConversionProfile profile)
        {
            foreach (var action in _actions)
            {
                if (action.IsEnabled(profile) && action.IsRestricted(profile))
                    return true;
            }
            return false;
        }

        private ActionResult ProfileCheck(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var actionResult = CheckFileNameAndTargetDirectory(profile, checkLevel);
            actionResult.AddRange(CheckOutputFormatForAutoMerge(profile));

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
                    _logger.Error("The path in OutputFilenameTemplate is null or empty.");
                    return new ActionResult(ErrorCode.FilePath_NullOrEmpty);

                case PathUtilStatus.InvalidRootedPath:
                    _logger.Error($"The path in OutputFilenameTemplate '{outputFilenameTemplate}' is not a valid rooted path.");
                    return new ActionResult(ErrorCode.FilePath_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    _logger.Error("The path in OutputFilenameTemplate is too long.");
                    return new ActionResult(ErrorCode.FilePath_TooLong);

                case PathUtilStatus.NotSupportedEx:
                    _logger.Error($"The path in OutputFilenameTemplate '{outputFilenameTemplate}' is not a valid rooted path.");
                    return new ActionResult(ErrorCode.FilePath_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    _logger.Error($"The path in OutputFilenameTemplate '{outputFilenameTemplate}' contains invalid characters.");
                    return new ActionResult(ErrorCode.FilePath_InvalidCharacters);

                case PathUtilStatus.Success:
                    break;
            }

            return new ActionResult();
        }

        private ActionResult CheckOutputFormatForAutoMerge(ConversionProfile profile)
        {
            if (!profile.AutoSave.Enabled)
                return new ActionResult();

            if (profile.AutoSave.ExistingFileBehaviour == AutoSaveExistingFileBehaviour.Merge && !profile.OutputFormat.IsPdf())
                return new ActionResult(ErrorCode.AutoSave_NonPdfAutoMerge);

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

            if (!profile.UserTokens.Enabled)
            {
                if (TokenIdentifier.ContainsUserToken(profile.TargetDirectory))
                    return new ActionResult(ErrorCode.TargetDirectory_RequiresUserTokens);
            }

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

            if (!profile.UserTokens.Enabled)
            {
                if (TokenIdentifier.ContainsUserToken(profile.FileNameTemplate)) 
                    return new ActionResult(ErrorCode.FilenameTemplate_RequiresUserTokens);
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
