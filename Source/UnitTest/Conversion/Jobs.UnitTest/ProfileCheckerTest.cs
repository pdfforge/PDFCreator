using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs
{
    [TestFixture]
    internal class ProfileCheckerTest
    {
        private TimeServerAccount _timeServerAccount;
        private ConversionProfile _profile;
        private IPathUtil _pathUtil;
        private IFile _file;
        private ProfileChecker _profileChecker;
        private List<ICheckable> _actionCheckList;
        private Accounts _accounts;
        private const string Token = "<Token>";
        private const string TokenValue = "TokenValue";

        [SetUp]
        public void SetUp()
        {
            _profile = new ConversionProfile();
            _profile.Name = "TestProfile";
            _profile.OutputFormat = OutputFormat.Pdf;
            _profile.AutoSave.Enabled = false;
            _profile.TargetDirectory = "directory";
            _profile.FileNameTemplate = "filename";
            _profile.PdfSettings.Signature.CertificateFile = "certfile";

            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.IsValidFilename(Arg.Any<string>()).Returns(true);
            _pathUtil.IsValidRootedPathWithResponse(Arg.Any<string>()).Returns(PathUtilStatus.Success);

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);

            _actionCheckList = new List<ICheckable>();
            _actionCheckList.Add(Substitute.For<ICheckable>());
            _actionCheckList.Add(Substitute.For<ICheckable>());
            _actionCheckList.Add(Substitute.For<ICheckable>());

            _accounts = new Accounts();
            _timeServerAccount = new TimeServerAccount();
            _timeServerAccount.AccountId = "TimeServerTestId";
            _profile.PdfSettings.Signature.TimeServerAccountId = _timeServerAccount.AccountId;
            _accounts.TimeServerAccounts.Add(_timeServerAccount);

            for (var i = 0; i < _actionCheckList.Count; i++)
            {
                _actionCheckList[i].Check(Arg.Any<ConversionProfile>(), _accounts, Arg.Any<CheckLevel>()).Returns(new ActionResult());
            }

            _profileChecker = new ProfileChecker(_pathUtil, _file, _actionCheckList);
        }

        private ActionResult GetFirstCheckProfileListResult()
        {
            var profiles = new List<ConversionProfile>(new[] { _profile });
            var resultDict = _profileChecker.CheckProfileList(profiles, _accounts);
            return resultDict ? new ActionResult() : resultDict[_profile.Name];
        }

        private ActionResult GetCheckJobResult(string outputFilenameTemplate = "")
        {
            var job = new Job(new JobInfo(), _profile, _accounts);
            job.OutputFileTemplate = outputFilenameTemplate;
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken(Token.Replace("<", "").Replace(">", ""), TokenValue);
            job.TokenReplacer = tokenReplacer;
            return _profileChecker.CheckJob(job);
        }

        #region General

        [Test]
        public void DefaultProfile_CheckLevelProfile_ReturnsTrue()
        {
            _profile = new ConversionProfile();

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Default profile has errors:" + Environment.NewLine + result);
        }

        [Test]
        public void DefaultProfile_CheckLevelJob_ReturnsTrue()
        {
            _profile = new ConversionProfile();

            var result = GetCheckJobResult();

            Assert.IsTrue(result, "Default profile has errors:" + Environment.NewLine + result);
        }

        [Test]
        public void CheckProfileList_CallsCheckOfAllActions()
        {
            var profile1 = new ConversionProfile { Name = "P1" };
            var profile2 = new ConversionProfile { Name = "P2" };
            var profile3 = new ConversionProfile { Name = "P3" };
            var profileList = new List<ConversionProfile>(new[] { profile1, profile2, profile3 });

            var expectedCodes = new Dictionary<ICheckable, ErrorCode>();
            var errorCodes = Enum.GetValues(typeof(ErrorCode)).Cast<ErrorCode>().ToList();

            foreach (var check in _actionCheckList)
            {
                var expectedCode = errorCodes.Skip(_actionCheckList.IndexOf(check)).First();
                check.Check(Arg.Any<ConversionProfile>(), _accounts, CheckLevel.Profile).Returns(new ActionResult(expectedCode));
                expectedCodes[check] = expectedCode;
            }

            var resultDict = _profileChecker.CheckProfileList(profileList, _accounts);

            foreach (var profile in profileList)
            {
                var result = resultDict[profile.Name]; //ResultDictonary must contain result for every profile name
                foreach (var check in _actionCheckList)
                {
                    check.Received().Check(profile, _accounts, CheckLevel.Profile); //Check must have been called for every profile
                    check.DidNotReceive().ApplyPreSpecifiedTokens(Arg.Any<Job>()); // Apply tokens must not be called
                    Assert.IsTrue(result.Contains(expectedCodes[check])); //result for profile must contain every expected check result
                }
            }
        }

        [Test]
        public void CheckJob_CallsApplyTokensAndCheckOfAllActions()
        {
            var expectedCodes = new Dictionary<ICheckable, ErrorCode>();
            var errorCodes = Enum.GetValues(typeof(ErrorCode)).Cast<ErrorCode>().ToList();

            foreach (var action in _actionCheckList)
            {
                var expectedCode = errorCodes.Skip(_actionCheckList.IndexOf(action)).First();
                action.Check(_profile, _accounts, CheckLevel.Job).Returns(new ActionResult(expectedCode));
                expectedCodes[action] = expectedCode;
            }

            var job = new Job(null, _profile, _accounts);

            var result = _profileChecker.CheckJob(job);

            foreach (var check in _actionCheckList)
            {
                check.Received().Check(job.Profile, _accounts, CheckLevel.Job);
                check.Received().ApplyPreSpecifiedTokens(job);
                Assert.IsTrue(result.Contains(expectedCodes[check])); //result for profile must contain every expected check result
            }
        }

        [Test]
        public void CheckJob_AppliesTokensForCoverFile()
        {
            _profile.CoverPage.File = Token;

            GetCheckJobResult();

            Assert.AreEqual(TokenValue, _profile.CoverPage.File);
        }

        [Test]
        public void CheckJob_AppliesTokensForAttachmentFile()
        {
            _profile.AttachmentPage.File = Token;

            GetCheckJobResult();

            Assert.AreEqual(TokenValue, _profile.AttachmentPage.File);
        }

        [Test]
        public void CheckJob_AppliesTokensForBackgroundFile()
        {
            _profile.BackgroundPage.File = Token;

            GetCheckJobResult();

            Assert.AreEqual(TokenValue, _profile.BackgroundPage.File);
        }

        [Test]
        public void CheckJob_AppliesTokensForCertificateFile()
        {
            _profile.PdfSettings.Signature.CertificateFile = Token;

            GetCheckJobResult();

            Assert.AreEqual(TokenValue, _profile.PdfSettings.Signature.CertificateFile);
        }

        #endregion General

        #region TargetDirectory

        [Test]
        public void TargetDirectory_NoAutoSave_NoTargetDirectory_ResultisTrue()
        {
            _profile.AutoSave.Enabled = false;
            _profile.TargetDirectory = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void TargetDirectory_AutoSave_NoTargetDirectory_ReturnsErrorCode()
        {
            _profile.AutoSave.Enabled = true;
            _profile.TargetDirectory = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.TargetDirectory_NotSetForAutoSave, result, "CheckJob did not detect missing target directory for autosave.");
            result.Remove(ErrorCode.TargetDirectory_NotSetForAutoSave);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void TargetDirectory_CheckLevelProfile_DirectoryContainsToken_ResultIsTrue()
        {
            _profile.TargetDirectory = "<Token>";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
            _pathUtil.DidNotReceive().IsValidRootedPathWithResponse(_profile.TargetDirectory);
        }

        [Test]
        public void TargetDirectory_CheckLevelJob_InvalidTargetDirectory_ResultIsTrue()
        {
            _profile.TargetDirectory = ": ? > < * should be ignored";

            var result = GetCheckJobResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void TargetDirectory_PathUtilStatusIsInvalidRootedPath_ReturnsErrorCode()
        {
            _profile.TargetDirectory = @"\\+*  .d..";
            _pathUtil.IsValidRootedPathWithResponse(_profile.TargetDirectory).Returns(PathUtilStatus.InvalidRootedPath);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.TargetDirectory_InvalidRootedPath, result, "CheckJob did not detect invalid rooted target directory path.");
            result.Remove(ErrorCode.TargetDirectory_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void TargetDirectory_PathUtilStatusIsPathTooLongEx_ReturnsErrorCode()
        {
            _profile.TargetDirectory = @"\\+*  .d..";
            _pathUtil.IsValidRootedPathWithResponse(_profile.TargetDirectory).Returns(PathUtilStatus.PathTooLongEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.TargetDirectory_TooLong, result, "CheckJob did not detect too long target directory path.");
            result.Remove(ErrorCode.TargetDirectory_TooLong);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void TargetDirectory_PathUtilStatusIsNotSupportedEx_ReturnsErrorCode()
        {
            _profile.TargetDirectory = @"\\+*  .d..";
            _pathUtil.IsValidRootedPathWithResponse(_profile.TargetDirectory).Returns(PathUtilStatus.NotSupportedEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.TargetDirectory_InvalidRootedPath, result, "CheckJob did not detect invalid rooted target directory path.");
            result.Remove(ErrorCode.TargetDirectory_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void TargetDirectory_PathUtilStatusIsArgumentEx_ReturnsErrorCode()
        {
            _profile.TargetDirectory = @"\\+*  .d..";
            _pathUtil.IsValidRootedPathWithResponse(_profile.TargetDirectory).Returns(PathUtilStatus.ArgumentEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.TargetDirectory_IllegalCharacters, result, "CheckJob did not detect illegal characters in target directory path.");
            result.Remove(ErrorCode.TargetDirectory_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion TargetDirectory

        #region FileNameTemplate

        [Test]
        public void FileNameTemplate_AutoSave_EmptyFileNameTemplate_ReturnsErrorCode()
        {
            _profile.AutoSave.Enabled = true;
            _profile.FileNameTemplate = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AutoSave_NoFilenameTemplate, result, "Profile check did not detect missing FileNameTemplate for AutoSave");
            result.Remove(ErrorCode.AutoSave_NoFilenameTemplate);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FileNameTemplate_NoAutoSave_EmptyFileNameTemplate_CallsPathUtilIsValidFileName()
        {
            _profile.AutoSave.Enabled = false;
            _profile.FileNameTemplate = "";

            GetFirstCheckProfileListResult();

            _pathUtil.Received().IsValidFilename(_profile.FileNameTemplate);
        }

        [Test]
        public void FileNameTemplate_CheckLevelProfile_FinaleNameTemplateContainsToken_ResultIsTrue()
        {
            _profile.FileNameTemplate = "<Token>";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
            _pathUtil.DidNotReceive().IsValidFilename(_profile.FileNameTemplate);
        }

        [Test]
        public void FileNameTemplate_CheckLevelJob_InvalidFinaleNameTemplate_ResultIsTrue()
        {
            _profile.FileNameTemplate = ": ? > < * should be ignored";

            var result = GetCheckJobResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FileNameTemplate_PathUtilIsValidFilenameReturnsFalse_ReturnsErrorCode()
        {
            _profile.FileNameTemplate = "InvalidFinaleName";
            _pathUtil.IsValidFilename(_profile.FileNameTemplate).Returns(false);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.FilenameTemplate_IllegalCharacters, result, "Profile check did not detect illegal characters in filename template");
            result.Remove(ErrorCode.FilenameTemplate_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion FileNameTemplate

        #region JobOutputFilenameTemplate

        [Test]
        public void JobOutputFilenameTemplate_CheckLevelJob_ValidRootedPath_ResultIsTrue()
        {
            var outputFilenameTemplate = "OutputFileTemplate";
            _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate).Returns(PathUtilStatus.Success);

            var result = GetCheckJobResult(outputFilenameTemplate);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void JobOutputFilenameTemplate_CheckLevelJob_IsEmpty_ReturnsErrorCode()
        {
            var outputFilenameTemplate = "";
            _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate).Returns(PathUtilStatus.PathWasNullOrEmpty);

            var result = GetCheckJobResult(outputFilenameTemplate);

            Assert.Contains(ErrorCode.FilePath_InvalidRootedPath, result);
            result.Remove(ErrorCode.FilePath_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void JobOutputFilenameTemplate_CheckLevelJob_IsInvalidRootedPath_ReturnsErrorCode()
        {
            var outputFilenameTemplate = "ABC:\\";
            _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate).Returns(PathUtilStatus.InvalidRootedPath);

            var result = GetCheckJobResult(outputFilenameTemplate);

            Assert.Contains(ErrorCode.FilePath_InvalidRootedPath, result);
            result.Remove(ErrorCode.FilePath_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void JobOutputFilenameTemplate_CheckLevelJob_IsTooLong_ReturnsErrorCode()
        {
            var outputFilenameTemplate = "TOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO...LONG";
            _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate).Returns(PathUtilStatus.PathTooLongEx);

            var result = GetCheckJobResult(outputFilenameTemplate);

            Assert.Contains(ErrorCode.FilePath_TooLong, result);
            result.Remove(ErrorCode.FilePath_TooLong);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void JobOutputFilenameTemplate_CheckLevelJob_IsNotSupported_ReturnsErrorCode()
        {
            var outputFilenameTemplate = "B:\\IsNotAvailable";
            _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate).Returns(PathUtilStatus.NotSupportedEx);

            var result = GetCheckJobResult(outputFilenameTemplate);

            Assert.Contains(ErrorCode.FilePath_InvalidRootedPath, result);
            result.Remove(ErrorCode.FilePath_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void JobOutputFilenameTemplate_CheckLevelJob_InvalidChars_ReturnsErrorCode()
        {
            var outputFilenameTemplate = "B:\\<>";
            _pathUtil.IsValidRootedPathWithResponse(outputFilenameTemplate).Returns(PathUtilStatus.ArgumentEx);

            var result = GetCheckJobResult(outputFilenameTemplate);

            Assert.Contains(ErrorCode.FilePath_InvalidCharacters, result);
            result.Remove(ErrorCode.FilePath_InvalidCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion JobOutputFilenameTemplate

        #region CoverPage

        [Test]
        public void CoverPage_CoverPageIsDisabled_ReturnsTrue()
        {
            _profile.CoverPage.Enabled = false;
            _profile.CoverPage.File = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_NoCoverFile_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Cover_NoFileSpecified, result, "Profile check did not detect missing cover file.");
            result.Remove(ErrorCode.Cover_NoFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_CheckLevelProfile_CoverFileContainsTokens_ReturnsTrue()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "<Token>";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_CheckLevelJob_CoverFileContainsToken_CheckContinuesWithNextCheck()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "<Token>.noPdf";

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Cover_NoPdf, result, "Profile check did not detect, that the cover file is no PDF.");
        }

        [Test]
        public void CoverPage_CoverFileIsNoPDF_ReturnsErrorResult()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "testfile.noPdf";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Cover_NoPdf, result, "Profile check did not detect, that the cover file is no PDF.");
            result.Remove(ErrorCode.Cover_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_PathUtilStatusIsInvlaidRootedPath_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.CoverPage.File).Returns(PathUtilStatus.InvalidRootedPath);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CoverPage_InvalidRootedPath, result, "CheckJob did not detect invalid rooted cover file.");
            result.Remove(ErrorCode.CoverPage_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_PathUtilStatusIsPathTooLongEx_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.CoverPage.File).Returns(PathUtilStatus.PathTooLongEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CoverPage_TooLong, result, "CheckJob did not detect too long cover file.");
            result.Remove(ErrorCode.CoverPage_TooLong);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_PathUtilStatusIsNotSupportedEx_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.CoverPage.File).Returns(PathUtilStatus.NotSupportedEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CoverPage_InvalidRootedPath, result, "CheckJob did not detect invalid rooted cover file.");
            result.Remove(ErrorCode.CoverPage_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_PathUtilStatusIsArgumentEx_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.CoverPage.File).Returns(PathUtilStatus.ArgumentEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CoverPage_IllegalCharacters, result, "CheckJob did not detect illegal characters in cover file.");
            result.Remove(ErrorCode.CoverPage_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_CheckLevelProfile_NotExistingCoverFileInNetworkPath_ReturnsTrue()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\networkpath\file.pdf";
            _file.Exists(_profile.CoverPage.File).Returns(false);

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_CheckLevelJob_NotExistingCoverFileInNetworkPath_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\networkpath\file.pdf";
            _file.Exists(_profile.CoverPage.File).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Cover_FileDoesNotExist, result, "CheckJob did not detect not existing cover file.");
            result.Remove(ErrorCode.Cover_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_NotExistingCoverFile_ReturnsErrorCode()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\not existing.pdf";
            _file.Exists(_profile.CoverPage.File).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Cover_FileDoesNotExist, result, "CheckJob did not detect not existing cover file.");
            result.Remove(ErrorCode.Cover_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPage_ExistingCoverFile_ReturnsTrue()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\not existing.pdf";
            _file.Exists(_profile.CoverPage.File).Returns(true);

            var result = GetCheckJobResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion CoverPage

        #region AttachmentPage

        [Test]
        public void AttachmentPage_AttachmentPageIsDisabled_ReturnsTrue()
        {
            _profile.AttachmentPage.Enabled = false;
            _profile.AttachmentPage.File = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_NoAttachmentFile_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Attachment_NoFileSpecified, result, "Profile check did not detect missing Attachment file.");
            result.Remove(ErrorCode.Attachment_NoFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_CheckLevelProfile_AttachmentFileContainsTokens_ReturnsTrue()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "<Token>";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_CheckLevelJob_AttachmentFileContainsToken_CheckContinuesWithNextCheck()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "<Token>.noPdf";

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Attachment_NoPdf, result, "Profile check did not detect, that the Attachment file is no PDF.");
        }

        [Test]
        public void AttachmentPage_AttachmentFileIsNoPDF_ReturnsErrorResult()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "testfile.noPdf";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Attachment_NoPdf, result, "Profile check did not detect, that the Attachment file is no PDF.");
            result.Remove(ErrorCode.Attachment_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_PathUtilStatusIsInvlaidRootedPath_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.AttachmentPage.File).Returns(PathUtilStatus.InvalidRootedPath);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AttachmentPage_InvalidRootedPath, result, "CheckJob did not detect invalid rooted Attachment file.");
            result.Remove(ErrorCode.AttachmentPage_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_PathUtilStatusIsPathTooLongEx_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.AttachmentPage.File).Returns(PathUtilStatus.PathTooLongEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AttachmentPage_TooLong, result, "CheckJob did not detect too long Attachment file.");
            result.Remove(ErrorCode.AttachmentPage_TooLong);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_PathUtilStatusIsNotSupportedEx_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.AttachmentPage.File).Returns(PathUtilStatus.NotSupportedEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AttachmentPage_InvalidRootedPath, result, "CheckJob did not detect invalid rooted Attachment file.");
            result.Remove(ErrorCode.AttachmentPage_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_PathUtilStatusIsArgumentEx_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.AttachmentPage.File).Returns(PathUtilStatus.ArgumentEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AttachmentPage_IllegalCharacters, result, "CheckJob did not detect illegal characters in Attachment file.");
            result.Remove(ErrorCode.AttachmentPage_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_CheckLevelProfile_NotExistingAttachmentFileInNetworkPath_ReturnsTrue()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\networkpath\file.pdf";
            _file.Exists(_profile.AttachmentPage.File).Returns(false);

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_CheckLevelJob_NotExistingAttachmentFileInNetworkPath_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\networkpath\file.pdf";
            _file.Exists(_profile.AttachmentPage.File).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Attachment_FileDoesNotExist, result, "CheckJob did not detect not existing Attachment file.");
            result.Remove(ErrorCode.Attachment_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_NotExistingAttachmentFile_ReturnsErrorCode()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\not existing.pdf";
            _file.Exists(_profile.AttachmentPage.File).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Attachment_FileDoesNotExist, result, "CheckJob did not detect not existing Attachment file.");
            result.Remove(ErrorCode.Attachment_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPage_ExistingAttachmentFile_ReturnsTrue()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\not existing.pdf";
            _file.Exists(_profile.AttachmentPage.File).Returns(true);

            var result = GetCheckJobResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion AttachmentPage

        #region BackgroundPage

        [Test]
        public void BackgroundPage_BackgroundPageIsDisabled_ReturnsTrue()
        {
            _profile.BackgroundPage.Enabled = false;
            _profile.BackgroundPage.File = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_BackgroundPageEnabled_OutputFormatIsNoPdf_ReturnsTrue()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "";
            _profile.OutputFormat = OutputFormat.Txt;

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_NoBackgroundFile_ReturnsErrorCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Background_NoFileSpecified, result, "Profile check did not detect missing Background file.");
            result.Remove(ErrorCode.Background_NoFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_CheckLevelProfile_BackgroundFileContainsTokens_ReturnsTrue()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "<Token>";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_CheckLevelJob_BackgroundFileContainsToken_CheckContinuesWithNextCheck()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "<Token>.noPdf";

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Background_NoPdf, result, "Profile check did not detect, that the Background file is no PDF.");
        }

        [Test]
        public void BackgroundPage_BackgroundFileIsNoPDF_ReturnsErrorResult()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "testfile.noPdf";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Background_NoPdf, result, "Profile check did not detect, that the Background file is no PDF.");
            result.Remove(ErrorCode.Background_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_PathUtilStatusIsInvlaidRootedPath_ReturnsErrorCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.BackgroundPage.File).Returns(PathUtilStatus.InvalidRootedPath);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.BackgroundPage_InvalidRootedPath, result, "CheckJob did not detect invalid rooted Background file.");
            result.Remove(ErrorCode.BackgroundPage_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_PathUtilStatusIsPathTooLongEx_ReturnsErrorCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.BackgroundPage.File).Returns(PathUtilStatus.PathTooLongEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.BackgroundPage_TooLong, result, "CheckJob did not detect too long Background file.");
            result.Remove(ErrorCode.BackgroundPage_TooLong);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_PathUtilStatusIsNotSupportedEx_ReturnsErrorCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.BackgroundPage.File).Returns(PathUtilStatus.NotSupportedEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.BackgroundPage_InvalidRootedPath, result, "CheckJob did not detect invalid rooted Background file.");
            result.Remove(ErrorCode.BackgroundPage_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_PathUtilStatusIsArgumentEx_ReturnsErrorCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.BackgroundPage.File).Returns(PathUtilStatus.ArgumentEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.BackgroundPage_IllegalCharacters, result, "CheckJob did not detect illegal characters in Background file.");
            result.Remove(ErrorCode.BackgroundPage_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_CheckLevelProfile_NotExistingBackgroundFileInNetworkPath_ReturnsTrue()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\networkpath\file.pdf";
            _file.Exists(_profile.BackgroundPage.File).Returns(false);

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_CheckLevelJob_NotExistingBackgroundFileInNetworkPath_ReturnsErrorCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\networkpath\file.pdf";
            _file.Exists(_profile.BackgroundPage.File).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Background_FileDoesNotExist, result, "CheckJob did not detect not existing Background file.");
            result.Remove(ErrorCode.Background_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_NotExistingBackgroundFile_ReturnsErroCode()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\not existing.pdf";
            _file.Exists(_profile.BackgroundPage.File).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.Background_FileDoesNotExist, result, "CheckJob did not detect not existing Background file.");
            result.Remove(ErrorCode.Background_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPage_ExistingBackgroundFile_ReturnsTrue()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\not existing.pdf";
            _file.Exists(_profile.BackgroundPage.File).Returns(true);

            var result = GetCheckJobResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion BackgroundPage

        #region Stamping

        [Test]
        public void Stamping_Disabled_ReturnsTrue()
        {
            _profile.Stamping.Enabled = false;
            _profile.Stamping.StampText = "";
            _profile.Stamping.FontName = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Stamping_NoFontname_ReturnsErrorCode()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "StampText";
            _profile.Stamping.FontName = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Stamp_NoFont, result, "Profile check did not detect missing font name.");
            result.Remove(ErrorCode.Stamp_NoFont);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Stamping_NoStampText_ReturnsErrorCode()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "";
            _profile.Stamping.FontName = "Arial";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Stamp_NoText, result, "Profile check did not detect missing stamp text.");
            result.Remove(ErrorCode.Stamp_NoText);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Stamping_ValidSettings_ReturnsTrue()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "Stamp Text";
            _profile.Stamping.FontName = "Arial";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Stamping_AllErrors()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "";
            _profile.Stamping.FontName = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Stamp_NoText, result, "Profile check did not detect missing stamp text.");
            result.Remove(ErrorCode.Stamp_NoText);
            Assert.Contains(ErrorCode.Stamp_NoFont, result, "Profile check did not detect missing font name.");
            result.Remove(ErrorCode.Stamp_NoFont);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Stamping

        #region Encryption

        [Test]
        public void Security_SecurityDisabled_ReturnsTrue()
        {
            _profile.PdfSettings.Security.Enabled = false;
            _profile.OutputFormat = OutputFormat.Pdf;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Security.OwnerPassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Security_OutputFormatIsNoPdf_ReturnsTrue()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.OutputFormat = OutputFormat.Txt;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Security.OwnerPassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Security_AutoSave_NoOwnerPassword_ReturnsErrorCode()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Security.OwnerPassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AutoSave_NoOwnerPassword, result, "CheckJob did not detect, missing owner password for autosave.");
            result.Remove(ErrorCode.AutoSave_NoOwnerPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Security_AutoSave_UserPwEnabled_NoUserPassword_ReturnsErrorCode()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Security.OwnerPassword = "OwnerPw";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AutoSave_NoUserPassword, result, "CheckJob did not detect, missing user password for autosave.");
            result.Remove(ErrorCode.AutoSave_NoUserPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Security_AutoSave_MultipleErrors()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.AutoSave_NoOwnerPassword, result, "CheckJob did not detect, missing owner password for autosave.");
            result.Remove(ErrorCode.AutoSave_NoOwnerPassword);
            Assert.Contains(ErrorCode.AutoSave_NoUserPassword, result, "CheckJob did not detect, missing user password for autosave.");
            result.Remove(ErrorCode.AutoSave_NoUserPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Security_ValidSettings_ReturnsTrue()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Security_ValidAutoSaveSettings_ReturnsTrue()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Security.OwnerPassword = "OwnerPw";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "UserPw";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Encryption

        #region Signature

        [Test]
        public void Signature_Disabled_ReturnsTrue()
        {
            _profile.PdfSettings.Signature.Enabled = false;
            _profile.PdfSettings.Signature.CertificateFile = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_OutputformatIsNotPdf_ReturnsTrue()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.OutputFormat = OutputFormat.Txt;
            _profile.PdfSettings.Signature.CertificateFile = "";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_AutoSave_NoCertificatePassword_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.AutoSave.Enabled = true;
            _profile.PdfSettings.Signature.SignaturePassword = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Signature_AutoSaveWithoutCertificatePassword, result, "Did not detect missing certificate password for AutoSave");
            result.Remove(ErrorCode.Signature_AutoSaveWithoutCertificatePassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_InvalidTimeServerAccount_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.TimeServerAccountId = "Invalid ID";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Signature_NoTimeServerAccount, result, "Did not detect missing TimeServerAccount");
            result.Remove(ErrorCode.Signature_NoTimeServerAccount);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_SecuredTimeServer_NoUserName_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "PW";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Signature_SecuredTimeServerWithoutUsername, result, "Did not detect missing UserName for secured time server");
            result.Remove(ErrorCode.Signature_SecuredTimeServerWithoutUsername);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_SecuredTimeServer_NoPassword_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "UserName";
            _timeServerAccount.Password = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.Signature_SecuredTimeServerWithoutPassword, result, "Did not detect missing Password for secured time server");
            result.Remove(ErrorCode.Signature_SecuredTimeServerWithoutPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_NoCertificateFile_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "";

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.ProfileCheck_NoCertificationFile, result, "Did not detect missing certificate file");
            result.Remove(ErrorCode.ProfileCheck_NoCertificationFile);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_CheckLevelProfile_CertificateFileContainsToken_ReturnsTrue()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "<Token>";

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
            _pathUtil.DidNotReceive().IsValidRootedPathWithResponse(_profile.PdfSettings.Signature.CertificateFile);
        }

        [Test]
        public void Signature_CheckLevelProfile_CertificateFileContains_CallsPathUtileIsValidRootedPathWithResponse()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "<Token>";

            GetCheckJobResult();

            _pathUtil.Received().IsValidRootedPathWithResponse(_profile.PdfSettings.Signature.CertificateFile);
        }

        [Test]
        public void Signature_PathUtilStatusIsInvalidRootedPathForCertFile_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.PdfSettings.Signature.CertificateFile).Returns(PathUtilStatus.InvalidRootedPath);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CertificateFile_InvalidRootedPath, result, "CheckJob did not detect invalid rooted certificate file path.");
            result.Remove(ErrorCode.CertificateFile_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_PathUtilStatusIsPathTooLongForCertFile_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.PdfSettings.Signature.CertificateFile).Returns(PathUtilStatus.PathTooLongEx);

            var result = GetFirstCheckProfileListResult();
            Assert.Contains(ErrorCode.CertificateFile_TooLong, result, "CheckJob did not detect too long certificate file path.");
            result.Remove(ErrorCode.CertificateFile_TooLong);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_PathUtilStatusIsNotSupportedExForCertFile_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.PdfSettings.Signature.CertificateFile).Returns(PathUtilStatus.NotSupportedEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CertificateFile_InvalidRootedPath, result, "CheckJob did not detect invalid rooted certificate file path.");
            result.Remove(ErrorCode.CertificateFile_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_PathUtilStatusIsArgumentExForCertFile_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\+*  .d..pdf";
            _pathUtil.IsValidRootedPathWithResponse(_profile.PdfSettings.Signature.CertificateFile).Returns(PathUtilStatus.ArgumentEx);

            var result = GetFirstCheckProfileListResult();

            Assert.Contains(ErrorCode.CertificateFile_IllegalCharacters, result, "CheckJob did not detect invalid rooted certificate file path.");
            result.Remove(ErrorCode.CertificateFile_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_CheckLevelProfile_CertificateFileIsNetworkPath_ReturnsTrue()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\certfile";
            _file.Exists(_profile.PdfSettings.Signature.CertificateFile).Returns(false);

            var result = GetFirstCheckProfileListResult();

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Signature_CheckLevelJob_CertificateFileDoesNotExist_ReturnsErrorCode()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\certfile";
            _file.Exists(_profile.PdfSettings.Signature.CertificateFile).Returns(false);

            var result = GetCheckJobResult();

            Assert.Contains(ErrorCode.CertificateFile_CertificateFileDoesNotExist, result, "Did not detect not existing certificate file path.");
            result.Remove(ErrorCode.CertificateFile_CertificateFileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Signature
    }
}
