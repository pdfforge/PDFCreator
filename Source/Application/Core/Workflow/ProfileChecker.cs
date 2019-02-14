using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IProfileChecker
    {
        ActionResultDict CheckProfileList(IList<ConversionProfile> profileList, Accounts accounts);

        ActionResult CheckJob(Job job);
    }

    public class ProfileChecker : IProfileChecker
    {
        private readonly IEnumerable<ICheckable> _actionChecks;
        private readonly IPathUtil _pathUtil;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProfileChecker(IPathUtil pathUtil, IFile file, IEnumerable<ICheckable> actionChecks)
        {
            _pathUtil = pathUtil;
            _file = file;
            _actionChecks = actionChecks;
        }

        public ActionResult CheckJob(Job job)
        {
            job.Profile.FileNameTemplate = job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate);
            job.Profile.CoverPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.CoverPage.File);
            job.Profile.AttachmentPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.AttachmentPage.File);
            job.Profile.BackgroundPage.File = job.TokenReplacer.ReplaceTokens(job.Profile.BackgroundPage.File);
            job.Profile.PdfSettings.Signature.CertificateFile = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.CertificateFile);

            foreach (var actionCheck in _actionChecks)
            {
                actionCheck.ApplyPreSpecifiedTokens(job);
            }

            var actionResult = CheckJobOutputFilenameTemplate(job.OutputFileTemplate);
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

        private ActionResult ProfileCheck(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            actionResult.AddRange(CheckTargetDirectory(profile, checkLevel));
            actionResult.AddRange(CheckFileNameTemplate(profile, checkLevel));
            actionResult.AddRange(CheckCoverPageSettings(profile, checkLevel));
            actionResult.AddRange(CheckAttachmentPageSettings(profile, checkLevel));
            actionResult.AddRange(CheckBackgroundPageSettings(profile, checkLevel));
            actionResult.AddRange(CheckStampingSettings(profile));
            actionResult.AddRange(CheckEncryptionSettings(profile));
            actionResult.AddRange(CheckSignatureSettings(profile, accounts, checkLevel));

            foreach (var actionCheck in _actionChecks)
            {
                var result = actionCheck.Check(profile, accounts, checkLevel);
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

        private ActionResult CheckCoverPageSettings(ConversionProfile profile, CheckLevel checkLevel)
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
                    return new ActionResult(ErrorCode.CoverPage_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.CoverPage_TooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.CoverPage_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.CoverPage_IllegalCharacters);
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

        private ActionResult CheckAttachmentPageSettings(ConversionProfile profile, CheckLevel checkLevel)
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
                _logger.Error("The attachment file \"" + profile.CoverPage.File + "\" is no pdf file.");
                return new ActionResult(ErrorCode.Attachment_NoPdf);
            }

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.AttachmentPage.File);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.AttachmentPage_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.AttachmentPage_TooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.AttachmentPage_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.AttachmentPage_IllegalCharacters);
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

        private ActionResult CheckBackgroundPageSettings(ConversionProfile profile, CheckLevel checkLevel)
        {
            if (!profile.BackgroundPage.Enabled)
                return new ActionResult();

            if (!profile.OutputFormat.IsPdf())
                return new ActionResult();

            if (string.IsNullOrEmpty(profile.BackgroundPage.File))
            {
                _logger.Error("No background file is specified.");
                return new ActionResult(ErrorCode.Background_NoFileSpecified);
            }

            var isJobLevelCheck = checkLevel == CheckLevel.Job;

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
                    return new ActionResult(ErrorCode.BackgroundPage_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.BackgroundPage_TooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.BackgroundPage_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.BackgroundPage_IllegalCharacters);
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

        private ActionResult CheckStampingSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if (profile.Stamping.Enabled)
            {
                if (string.IsNullOrEmpty(profile.Stamping.StampText))
                {
                    _logger.Error("No stamp text is specified.");
                    actionResult.Add(ErrorCode.Stamp_NoText);
                }
                if (string.IsNullOrEmpty(profile.Stamping.FontName))
                {
                    _logger.Error("No stamp font is specified.");
                    actionResult.Add(ErrorCode.Stamp_NoFont);
                }
            }
            return actionResult;
        }

        private ActionResult CheckEncryptionSettings(ConversionProfile profile)
        {
            var result = new ActionResult();

            var security = profile.PdfSettings.Security;

            if (!security.Enabled)
                return result;

            if (!profile.OutputFormat.IsPdf())
                return result;

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrEmpty(security.OwnerPassword))
                {
                    _logger.Error("No saved owner password for security in automatic saving.");
                    result.Add(ErrorCode.AutoSave_NoOwnerPassword);
                }

                if (security.RequireUserPassword)
                {
                    if (string.IsNullOrEmpty(security.UserPassword))
                    {
                        _logger.Error("No saved user password for security in automatic saving.");
                        result.Add(ErrorCode.AutoSave_NoUserPassword);
                    }
                }
            }

            return result;
        }

        private ActionResult CheckSignatureSettings(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            var signature = profile.PdfSettings.Signature;

            if (!signature.Enabled)
                return result;

            if (!profile.OutputFormat.IsPdf())
                return result;

            var isJobLevelCheck = checkLevel == CheckLevel.Job;

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrEmpty(signature.SignaturePassword))
                {
                    _logger.Error("Automatic saving without certificate password.");
                    result.Add(ErrorCode.Signature_AutoSaveWithoutCertificatePassword);
                }
            }

            var timeServerAccount = accounts.GetTimeServerAccount(profile);
            if (timeServerAccount == null)
            {
                _logger.Error("The specified time server account for signing is not configured.");
                result.Add(ErrorCode.Signature_NoTimeServerAccount);
            }
            else
            {
                if (timeServerAccount.IsSecured)
                {
                    if (string.IsNullOrEmpty(timeServerAccount.UserName))
                    {
                        _logger.Error("Secured Time Server without Login Name.");
                        result.Add(ErrorCode.Signature_SecuredTimeServerWithoutUsername);
                    }
                    if (string.IsNullOrEmpty(timeServerAccount.Password))
                    {
                        _logger.Error("Secured Time Server without Password.");
                        result.Add(ErrorCode.Signature_SecuredTimeServerWithoutPassword);
                    }
                }
            }

            var certificateFile = profile.PdfSettings.Signature.CertificateFile;

            if (string.IsNullOrEmpty(certificateFile))
            {
                _logger.Error("Error in signing. Missing certification file.");
                result.Add(ErrorCode.ProfileCheck_NoCertificationFile);
                return result;
            }

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(certificateFile))
                return result;

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.PdfSettings.Signature.CertificateFile);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    result.Add(ErrorCode.CertificateFile_InvalidRootedPath);
                    return result;

                case PathUtilStatus.PathTooLongEx:
                    result.Add(ErrorCode.CertificateFile_TooLong);
                    return result;

                case PathUtilStatus.NotSupportedEx:
                    result.Add(ErrorCode.CertificateFile_InvalidRootedPath);
                    return result;

                case PathUtilStatus.ArgumentEx:
                    result.Add(ErrorCode.CertificateFile_IllegalCharacters);
                    return result;
            }

            if (!isJobLevelCheck && certificateFile.StartsWith(@"\\"))
                return result;

            if (!_file.Exists(certificateFile))
            {
                _logger.Error("Error in signing. The certification file '" + certificateFile +
                              "' doesn't exist.");
                result.Add(ErrorCode.CertificateFile_CertificateFileDoesNotExist);
            }

            return result;
        }
    }
}
