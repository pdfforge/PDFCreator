using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class SigningAction : ActionBase<Signature>, IConversionAction
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
        private readonly IFontPathHelper _fontPathHelper;

        public SigningAction(IFile file, IPathUtil pathUtil, ISignaturePasswordCheck signaturePasswordCheck, IFontPathHelper fontPathHelper)
            : base(p => p.PdfSettings.Signature)
        {
            _file = file;
            _pathUtil = pathUtil;
            _signaturePasswordCheck = signaturePasswordCheck;
            _fontPathHelper = fontPathHelper;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            //nothing to do here. The Signing must be triggered as last processing step in the ActionExecutor
            return new ActionResult();
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
            var isJobLevelCheck = checkLevel == CheckLevel.RunningJob;

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

            var isImageRequired = (profile.PdfSettings.Signature.DisplaySignature == DisplaySignature.ImageOnly)
                                  || (profile.PdfSettings.Signature.DisplaySignature == DisplaySignature.ImageAndText);

            if (isImageRequired)
            {
                if (string.IsNullOrEmpty(profile.PdfSettings.Signature.BackgroundImageFile))
                {
                    _logger.Error("The signature image file \"" + profile.PdfSettings.Signature.BackgroundImageFile + "\" does not exist.");
                    result.Add(ErrorCode.Signature_ImageFileDoesNotExist);
                }
                else if (isJobLevelCheck || !TokenIdentifier.ContainsTokens(profile.PdfSettings.Signature.BackgroundImageFile))
                {
                    var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.PdfSettings.Signature.BackgroundImageFile);
                    switch (pathUtilStatus)
                    {
                        case PathUtilStatus.InvalidRootedPath:
                            result.Add(ErrorCode.Signature_ImageFileInvalidRootedPath);
                            break;

                        case PathUtilStatus.PathTooLongEx:
                            result.Add(ErrorCode.Signature_ImageFilePathTooLong);
                            break;

                        case PathUtilStatus.NotSupportedEx:
                            result.Add(ErrorCode.Signature_ImageFileUnsupportedType);
                            break;

                        case PathUtilStatus.ArgumentEx:
                            result.Add(ErrorCode.Signature_ImageFileIllegalCharacters);
                            break;

                        case PathUtilStatus.Success:
                            {
                                if (isJobLevelCheck || !profile.PdfSettings.Signature.BackgroundImageFile.StartsWith(@"\\"))
                                    if (!_file.Exists(profile.PdfSettings.Signature.BackgroundImageFile))
                                    {
                                        _logger.Error("The signature image file \"" + profile.PdfSettings.Signature.BackgroundImageFile + "\" does not exist.");
                                        result.Add(ErrorCode.Signature_ImageFileDoesNotExist);
                                    }
                            }
                            break;
                    }
                }
            }

            if (isJobLevelCheck)
                if (!_fontPathHelper.TryGetFontPath(profile.PdfSettings.Signature.FontFile, out _))
                    result.Add(ErrorCode.Signature_FontNotFound);

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
            return !profile.OutputFormat.IsPdf() || profile.OutputFormat == OutputFormat.PdfX;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        {
            if (job.Profile.OutputFormat == OutputFormat.PdfA1B)
                job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
        }
    }
}
