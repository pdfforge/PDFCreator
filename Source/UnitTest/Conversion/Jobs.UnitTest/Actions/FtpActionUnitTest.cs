using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class FtpActionUnitTest
    {
        private const string JobFtpPassword = "JobFtpPassword";
        private const string ProfileServer = "ProfileServer";
        private const string ProfileUserName = "ProfileUserName";

        private FtpAction _ftpAction;
        private IFtpConnection _ftpConnectionWrap;
        private IPathUtil _pathUtil;

        private Job _job;
        private ConversionProfile _profile;
        private Accounts _accounts;
        private FtpAccount _ftpTestAccount;

        private string _host;
        private string _userName;
        private string _password;

        [SetUp]
        public void Setup()
        {
            _ftpConnectionWrap = Substitute.For<IFtpConnection>();
            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.MAX_PATH.Returns(259);

            var factory = Substitute.For<IFtpConnectionFactory>();
            factory.BuildConnection(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(_ftpConnectionWrap).AndDoes(delegate (CallInfo info)
            {
                _host = info.ArgAt<string>(0);
                _userName = info.ArgAt<string>(1);
                _password = info.ArgAt<string>(2);
            });

            _ftpTestAccount = new FtpAccount();
            _ftpTestAccount.AccountId = "FtpTestAccountId";
            _ftpTestAccount.Server = ProfileServer;
            _ftpTestAccount.UserName = ProfileUserName;
            _ftpTestAccount.Password = "ProfileFtpPassword";

            _accounts = new Accounts();
            _accounts.FtpAccounts.Add(_ftpTestAccount);

            _profile = new ConversionProfile();
            _profile.Ftp.Enabled = true;
            _profile.Ftp.AccountId = _ftpTestAccount.AccountId;

            _ftpAction = new FtpAction(factory, _pathUtil);

            var jobPws = new JobPasswords();
            jobPws.FtpPassword = JobFtpPassword;
            _job = new Job(new JobInfo(), _profile, _accounts);
            _job.Accounts = _accounts;
            _job.Passwords = jobPws;
            _job.TokenReplacer = new TokenReplacer();
        }

        [Test]
        public void IsEnabled_ReturnsFtpEnabled()
        {
            _profile.Ftp.Enabled = true;
            Assert.IsTrue(_ftpAction.IsEnabled(_profile));
            _profile.Ftp.Enabled = false;
            Assert.IsFalse(_ftpAction.IsEnabled(_profile));
        }

        [Test]
        public void ApplyTokens_RepalcesTokensInFtpDirectoryAndMakesItValid()
        {
            var token = "<Token>";
            var tokenKey = "Token";
            var tokenValue = "TokenValue";
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken(tokenKey, tokenValue);
            _profile.Ftp.Directory = token + "?*";
            var job = new Job(null, _profile, _accounts);
            job.TokenReplacer = tokenReplacer;

            _ftpAction.ApplyPreSpecifiedTokens(job);

            Assert.AreEqual(tokenValue + "_", _profile.Ftp.Directory);
        }

        #region Check

        [Test]
        public void Check_FtpDisabled_RetunsTrue()
        {
            _profile.Ftp.Enabled = false;
            _profile.Ftp.AccountId = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + result);
        }

        [Test]
        public void Check_NoAutoSave_ValidSettings_RetunsTrue()
        {
            _profile.AutoSave.Enabled = false;
            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + result);
        }

        [Test]
        public void Check_AutoSave_ValidSettings_RetunsTrue()
        {
            _profile.AutoSave.Enabled = true;
            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + result);
        }

        [Test]
        public void Check_InvalidAccount_RetunsErrorCode()
        {
            _profile.Ftp.AccountId = "Some unavailble Account ID";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.AreEqual(ErrorCode.Ftp_NoAccount, result.FirstOrDefault());
        }

        [Test]
        public void Check_NoServer_RetunsErrorCode()
        {
            _ftpTestAccount.Server = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.AreEqual(ErrorCode.Ftp_NoServer, result.FirstOrDefault());
        }

        [Test]
        public void Check_NoUsername_RetunsErrorCode()
        {
            _ftpTestAccount.UserName = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.AreEqual(ErrorCode.Ftp_NoUser, result.FirstOrDefault());
        }

        [Test]
        public void Check_AutoSaveAndNoPassword_RetunsErrorCode()
        {
            _profile.AutoSave.Enabled = true;
            _ftpTestAccount.Password = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.AreEqual(ErrorCode.Ftp_AutoSaveWithoutPassword, result.FirstOrDefault());
        }

        [Test]
        public void Check_NoAutoSaveAndNoPassword_RetunsTrue()
        {
            _profile.AutoSave.Enabled = false;
            _ftpTestAccount.Password = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result);
        }

        [Test]
        public void Check_CheckLevelProfile_DirectoryContainsToken_RetunsTrue()
        {
            _profile.Ftp.Directory = "<Token>";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result);
        }

        [Test]
        public void Check_CheckLevelJob_DirectoryContainsToken_RetunsErrorCode()
        {
            _profile.Ftp.Directory = "<Token>";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Job);

            Assert.AreEqual(ErrorCode.FtpDirectory_InvalidFtpPath, result.FirstOrDefault());
        }

        [Test]
        public void Check_CheckLevelJob_NoAutoSave_AllErrors()
        {
            _profile.AutoSave.Enabled = false;
            _profile.Ftp.Directory = "<Token>";
            _ftpTestAccount.Server = "";
            _ftpTestAccount.UserName = "";
            //_ftpTestAccount.Password = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Job);

            Assert.Contains(ErrorCode.Ftp_NoServer, result, "CheckJob did not detect missing FTP server.");
            result.Remove(ErrorCode.Ftp_NoServer);
            Assert.Contains(ErrorCode.Ftp_NoUser, result, "CheckJob did not detect missing FTP username.");
            result.Remove(ErrorCode.Ftp_NoUser);
            Assert.Contains(ErrorCode.FtpDirectory_InvalidFtpPath, result, "CheckJob did not detect InvalidFtpPath");
            result.Remove(ErrorCode.FtpDirectory_InvalidFtpPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + result);
        }

        [Test]
        public void Check_CheckLevelJob_AutoSave_AllErrors()
        {
            _profile.AutoSave.Enabled = true;
            _profile.Ftp.Directory = "<Token>";
            _ftpTestAccount.Server = "";
            _ftpTestAccount.UserName = "";
            _ftpTestAccount.Password = "";

            var result = _ftpAction.Check(_profile, _accounts, CheckLevel.Job);

            Assert.Contains(ErrorCode.Ftp_NoServer, result, "CheckJob did not detect missing FTP server.");
            result.Remove(ErrorCode.Ftp_NoServer);
            Assert.Contains(ErrorCode.Ftp_NoUser, result, "CheckJob did not detect missing FTP username.");
            result.Remove(ErrorCode.Ftp_NoUser);
            Assert.Contains(ErrorCode.Ftp_AutoSaveWithoutPassword, result, "CheckJob did not detect AutoSaveWithoutPassword.");
            result.Remove(ErrorCode.Ftp_AutoSaveWithoutPassword);
            Assert.Contains(ErrorCode.FtpDirectory_InvalidFtpPath, result, "CheckJob did not detect InvalidFtpPath.");
            result.Remove(ErrorCode.FtpDirectory_InvalidFtpPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + result);
        }

        #endregion Check

        [Test]
        public void ProcessValidJob_CreateDirectoryThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.Directory = "D";
            _ftpConnectionWrap.When(x => x.CreateDirectory("D")).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Ftp_DirectoryError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_createFtpConnectionGetsCalledWithCorrectValues()
        {
            _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ProfileServer, _host);
            Assert.AreEqual(ProfileUserName, _userName);
            Assert.AreEqual(JobFtpPassword, _password);
        }

        [Test]
        public void ProcessValidJob_CreatesNonExistingDirectory()
        {
            var folder = "Folder";
            _profile.Ftp.Directory = folder;
            _ftpConnectionWrap.DirectoryExists(Arg.Any<string>()).Returns(false);

            _ftpAction.ProcessJob(_job);

            Received.InOrder(() =>
            {
                _ftpConnectionWrap.CreateDirectory(folder);
                _ftpConnectionWrap.SetCurrentDirectory(folder);
            });
        }

        [Test]
        public void ProcessValidJob_CreatesNonExistingSubDirectories()
        {
            var folder = "Folder";
            var subFolder1 = "Subfolder1";
            var subFolder2 = "SubFolder2";
            _profile.Ftp.Directory = folder + @"/" + subFolder1 + @"/" + subFolder2;
            _ftpConnectionWrap.DirectoryExists(folder).Returns(true);
            _ftpConnectionWrap.DirectoryExists(subFolder1).Returns(false);
            _ftpConnectionWrap.DirectoryExists(subFolder1).Returns(false);

            _ftpAction.ProcessJob(_job);

            Received.InOrder(() =>
            {
                _ftpConnectionWrap.CreateDirectory(subFolder1);
                _ftpConnectionWrap.SetCurrentDirectory(subFolder1);
                _ftpConnectionWrap.CreateDirectory(subFolder2);
                _ftpConnectionWrap.SetCurrentDirectory(subFolder2);
            });
        }

        [Test]
        public void ProcessValidJob_DirectoryExistThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.Directory = "D";
            _ftpConnectionWrap.When(x => x.DirectoryExists("D")).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Ftp_DirectoryError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_DoesNotCreatesExistingDirectory()
        {
            var folder = "Folder";
            _profile.Ftp.Directory = folder;
            _ftpConnectionWrap.DirectoryExists(Arg.Any<string>()).Returns(true);

            _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Test]
        public void ProcessValidJob_FtpLoginCalledAfterOpen_EachOnlyOnce()
        {
            _ftpAction.ProcessJob(_job);
            _ftpConnectionWrap.Received(1).Open();
            _ftpConnectionWrap.Received(1).Login();
            Received.InOrder(() =>
            {
                _ftpConnectionWrap.Open();
                _ftpConnectionWrap.Login();
            });
        }

        [Test]
        public void ProcessValidJob_FtpOpenThrowsException_CallFtpClose_ResultContainsAssociatedCode()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Ftp_LoginError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_FtpOpenThrowsWin32Exception_CallFtpClose_ResultContainsAssociatedCode()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Win32Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Ftp_LoginError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void
            ProcessValidJob_FtpOpenThrowsWin32ExceptionNativeErrorCode12007_CallFtpClose_ResultContainsAssociatedCode()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Win32Exception(12007); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Ftp_ConnectionError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_FtpPutFileThrowsExcpetion_CallFtpClose_ResultContainsAssociatedCode()
        {
            var outputFile = "file.pdf";
            _job.OutputFiles = new List<string>(new[] { outputFile });
            _ftpConnectionWrap.When(x => x.PutFile(Arg.Any<string>(), Arg.Any<string>()))
                .Do(x => { throw new Exception(); });

            var results = _ftpAction.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.Ftp_UploadError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_InvalidPathIsMadeValid()
        {
            var invalidPath = "Invalid " + new string(Path.GetInvalidPathChars()) + ":*? Path";
            _profile.Ftp.Directory = invalidPath;
            var validPath = "Invalid _ Path";

            _ftpAction.ProcessJob(_job);

            //Check first use of corrected path
            _ftpConnectionWrap.Received().DirectoryExists(validPath);
        }

        [Test]
        public void ProcessValidJob_MultipleOutputFiles_FtpPutFileGetsCalledMultipleTimes_CallFtpClose_ResultIstTrue()
        {
            var outputFile = "file.jpg";
            var outputFile2 = "file2.jpg";
            var outputFile3 = "file3.jpg";
            _job.OutputFiles = new List<string>(new[] { outputFile, outputFile2, outputFile3 });

            var results = _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.Received(1).PutFile(outputFile, outputFile);
            _ftpConnectionWrap.Received(1).PutFile(outputFile2, outputFile2);
            _ftpConnectionWrap.Received(1).PutFile(outputFile3, outputFile3);

            Assert.IsTrue(results);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_SetCurrentDirectoryThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.Directory = "D";
            _ftpConnectionWrap.When(x => x.SetCurrentDirectory("D")).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Ftp_DirectoryError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_TokenInDirectoryGetReplaced()
        {
            var tr = new TokenReplacer();
            tr.AddToken(new StringToken("token", "sometokenvalue"));
            _job.TokenReplacer = tr;

            _profile.Ftp.Directory = "<token>";
            var validPath = "sometokenvalue";

            _ftpAction.ProcessJob(_job);

            //Check first use of corrected path
            _ftpConnectionWrap.Received().DirectoryExists(validPath);
        }

        [Test]
        public void ProcessValidJobEnsureUniqueFileNames_FirstFileDoesExist_FilesGetAppendix()
        {
            _profile.Ftp.EnsureUniqueFilenames = true;
            var outputFile = "file.jpg";
            var outputFile2 = "file2.jpg";
            var outputFile3 = "file3.jpg";
            _job.OutputFiles = new List<string>(new[] { outputFile, outputFile2, outputFile3 });
            _ftpConnectionWrap.FileExists(outputFile).Returns(true);

            _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.Received(1).PutFile(outputFile, "file_2.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile2, "file2.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile3, "file3.jpg");
        }

        [Test]
        public void ProcessValidJobEnsureUniqueFileNames_FirstFileDoesExistFileWithAppendixDoesExist_FilesGetIncreasedAppendix()
        {
            _profile.Ftp.EnsureUniqueFilenames = true;
            var outputFile = "file.jpg";
            var outputFile2 = "file2.jpg";
            var outputFile3 = "file3.jpg";
            _job.OutputFiles = new List<string>(new[] { outputFile, outputFile2, outputFile3 });
            _ftpConnectionWrap.FileExists(outputFile2).Returns(true);
            _ftpConnectionWrap.FileExists("file2_2.jpg").Returns(true);

            _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.Received(1).PutFile(outputFile, "file.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile2, "file2_3.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile3, "file3.jpg");
        }

        [Test]
        public void
            ProcessValidJobEnsureUniqueFilenamesFtpFileExistsThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.EnsureUniqueFilenames = true;
            var outputFile = "file.pdf";
            _job.OutputFiles = new List<string>(new[] { outputFile });

            _ftpConnectionWrap.When(x => x.FileExists(Arg.Any<string>())).Do(x => { throw new Exception(); });

            var results = _ftpAction.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.Ftp_DirectoryReadError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_FtpOpenFailsWithWrongPassword_CallFtpClose_ResultsIsLoginError()
        {
            // raise error for wrong password
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Win32Exception(12014); });

            var result = _ftpAction.ProcessJob(_job);

            Assert.IsTrue(result.Contains(ErrorCode.PasswordAction_Login_Error));
            _ftpConnectionWrap.Received(1).Close();
        }
    }
}
