using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class SigningAction : ActionBase<Signature>, IConversionAction
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;

        public SigningAction(IFile file, IPathUtil pathUtil, ISignaturePasswordCheck signaturePasswordCheck)
            : base(p => p.PdfSettings.Signature)
        {
            _file = file;
            _pathUtil = pathUtil;
            _signaturePasswordCheck = signaturePasswordCheck;
        }

        protected override ActionResult DoProcessJob(Job job)
        {
            throw new NotImplementedException();
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            //nothing to do here. The Signing must be triggered as last processing step in the ActionManager
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.PdfSettings.Signature.CertificateFile = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.CertificateFile);

            job.Profile.PdfSettings.Signature.SignReason = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignReason);
            job.Profile.PdfSettings.Signature.SignContact = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignContact);
            job.Profile.PdfSettings.Signature.SignLocation = job.TokenReplacer.ReplaceTokens(job.Profile.PdfSettings.Signature.SignLocation);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!profile.PdfSettings.Signature.Enabled)
                return new ActionResult();
            if (!profile.OutputFormat.IsPdf())
                return new ActionResult();

            var (result, doPasswordCheck) = CheckCertificateFile(profile, checkLevel);

            var signature = profile.PdfSettings.Signature;

            if (string.IsNullOrEmpty(signature.SignaturePassword))
            {
                if (profile.AutoSave.Enabled)
                {
                    _logger.Error("Automatic saving without certificate password.");
                    result.Add(ErrorCode.Signature_AutoSaveWithoutCertificatePassword);
                }
            }
            else
            {
                //Skip PasswordCheck for Job to enhance performance
                if (doPasswordCheck && checkLevel == CheckLevel.EditingProfile)
                    if (!_signaturePasswordCheck.IsValidPassword(signature.CertificateFile, signature.SignaturePassword))
                        result.Add(ErrorCode.Signature_WrongCertificatePassword);
            }

            var timeServerAccount = settings.Accounts.GetTimeServerAccount(profile);
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

            return result;
        }

        private (ActionResult actionResult, bool doPasswordCheck) CheckCertificateFile(ConversionProfile profile, CheckLevel checkLevel)
        {
            var certificateFile = profile.PdfSettings.Signature.CertificateFile;

            if (string.IsNullOrEmpty(certificateFile))
            {
                _logger.Error("Error in signing. Missing certification file.");
                return (new ActionResult(ErrorCode.ProfileCheck_NoCertificationFile), false);
            }

            var isJobLevelCheck = checkLevel == CheckLevel.RunningJob;

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(certificateFile))
                return (new ActionResult(), false);

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.PdfSettings.Signature.CertificateFile);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return (new ActionResult(ErrorCode.CertificateFile_InvalidRootedPath), false);

                case PathUtilStatus.PathTooLongEx:
                    return (new ActionResult(ErrorCode.CertificateFile_TooLong), false);

                case PathUtilStatus.NotSupportedEx:
                    return (new ActionResult(ErrorCode.CertificateFile_InvalidRootedPath), false);

                case PathUtilStatus.ArgumentEx:
                    return (new ActionResult(ErrorCode.CertificateFile_IllegalCharacters), false);
            }

            if (!isJobLevelCheck && certificateFile.StartsWith(@"\\"))
                return (new ActionResult(), false);

            if (!_file.Exists(certificateFile))
            {
                _logger.Error("Error in signing. The certification file '" + certificateFile +
                              "' doesn't exist.");
                return (new ActionResult(ErrorCode.CertificateFile_CertificateFileDoesNotExist), false);
            }

            return (new ActionResult(), true);
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return !profile.OutputFormat.IsPdf();
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        {
            if (job.Profile.OutputFormat == OutputFormat.PdfA1B)
                job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
        }
    }
}
