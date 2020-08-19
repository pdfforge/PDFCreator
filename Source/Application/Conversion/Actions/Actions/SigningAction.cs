using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class SigningAction : IConversionAction, ICheckable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public SigningAction(IFile file, IPathUtil pathUtil)
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
            return profile.PdfSettings.Signature.Enabled;
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            //nothing to do here. The Signing must be triggered as last processing step in the ActionManager
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.PdfSettings.Signature.CertificateFile = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.CertificateFile);

            job.Profile.PdfSettings.Signature.SignReason = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignReason);
            job.Profile.PdfSettings.Signature.SignContact = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignContact);
            job.Profile.PdfSettings.Signature.SignLocation = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignLocation);
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
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
