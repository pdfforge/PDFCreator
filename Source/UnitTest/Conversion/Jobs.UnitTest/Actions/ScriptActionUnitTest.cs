using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class ScriptActionUnitTest
    {
        private ConversionProfile _profile;
        private IFile _file;
        private IPath _path;
        private IProcessStarter _processStarter;
        private IPathUtil _pathUtil;
        private ScriptAction _scriptAction;
        private readonly Accounts _unusedAccounts = null;

        [SetUp]
        public void Setup()
        {
            _profile = new ConversionProfile();

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);
            _processStarter = Substitute.For<IProcessStarter>();
            _path = Substitute.For<IPath>();
            _pathUtil = Substitute.For<IPathUtil>();
            _scriptAction = new ScriptAction(_path, _processStarter, _file, _pathUtil);
        }

        [Test]
        public void Check_ValidScriptSettings_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "ScriptfileDummy.exe";

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_ScriptActionDisabled_ResultIsTrue()
        {
            _profile.Scripting.Enabled = false;
            _profile.Scripting.ScriptFile = string.Empty;

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoScriptFile_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "";

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Script_NoScriptFileSpecified, result, "did not detect missing script file.");
            result.Remove(ErrorCode.Script_NoScriptFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_CheckLevelProfile_ScripFileContainsToken_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "Folder\\<Token>\\Script.exe";
            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
            //No further checks should be done
            _pathUtil.DidNotReceive().IsValidRootedPathWithResponse(Arg.Any<string>());
            _file.DidNotReceive().Exists(Arg.Any<string>());
        }

        [Test]
        public void Check_PathUtilDetectsInvalidRootedPath_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "For_PathUtil.exe";

            _pathUtil.IsValidRootedPathWithResponse(_profile.Scripting.ScriptFile).Returns(PathUtilStatus.InvalidRootedPath);
            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);
            Assert.Contains(ErrorCode.Script_InvalidRootedPath, result, "Did not detect InvalidRootedPath");
            result.Remove(ErrorCode.Script_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_PathUtilDetectsPathTooLongEx_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "For-PathUtil.exe";

            _pathUtil.IsValidRootedPathWithResponse(_profile.Scripting.ScriptFile).Returns(PathUtilStatus.PathTooLongEx);
            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);
            Assert.Contains(ErrorCode.Script_PathTooLong, result, "Did not detect PathTooLong");
            result.Remove(ErrorCode.Script_PathTooLong);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_PathUtilDetectsNotSupportedEx_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "For-PathUtil.exe";

            _pathUtil.IsValidRootedPathWithResponse(_profile.Scripting.ScriptFile).Returns(PathUtilStatus.NotSupportedEx);
            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);
            Assert.Contains(ErrorCode.Script_InvalidRootedPath, result, "Did not detect NotSupportedEx");
            result.Remove(ErrorCode.Script_InvalidRootedPath);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_PathUtilDetectsArgumentEx_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "For-PathUtil.exe";

            _pathUtil.IsValidRootedPathWithResponse(_profile.Scripting.ScriptFile).Returns(PathUtilStatus.ArgumentEx);
            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);
            Assert.Contains(ErrorCode.Script_IllegalCharacters, result, "Did not detect ArgumentEx");
            result.Remove(ErrorCode.Script_IllegalCharacters);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_ScriptFileDoesNotExists_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "A:\\NoNetworkPath\\Doesnotexist.exe";
            _file.Exists(_profile.Scripting.ScriptFile).Returns(false);

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Script_FileDoesNotExist, result, "Did not detect FileDoesNotExist");
            result.Remove(ErrorCode.Script_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_ScriptFileDoeExists_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "A:\\NoNetworkPath\\Exist.exe";
            _file.Exists(_profile.Scripting.ScriptFile).Returns(true);

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_CheckLevelProfile_ScriptFileInNetworkPath_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "\\\\NetworkPath\\Doesnotexist.exe";

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_CheckLevelJob_ScriptFileInNetworkPathDoesNotExist_ResultContainsErrorCode()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "\\\\NetworkPath\\Doesnotexist.exe";
            _file.Exists(_profile.Scripting.ScriptFile).Returns(false);

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Job);

            Assert.Contains(ErrorCode.Script_FileDoesNotExist, result, "Did not detect FileDoesNotExist");
            result.Remove(ErrorCode.Script_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_CheckLevelJob_ScriptFileInNetworkPathExists_ResultIsTrue()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "\\\\NetworkPath\\Doesnotexist.exe";
            _file.Exists(_profile.Scripting.ScriptFile).Returns(true);

            var result = _scriptAction.Check(_profile, _unusedAccounts, CheckLevel.Job);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void IsEnabled_ReturnsProfileScrtingEnabled()
        {
            _profile.Scripting.Enabled = true;
            Assert.IsTrue(_scriptAction.IsEnabled(_profile));

            _profile.Scripting.Enabled = false;
            Assert.IsFalse(_scriptAction.IsEnabled(_profile));
        }

        [Test]
        public void ComposeScript_PathGetFullPathThrowsExpection_DoNotThrowExcpetion_PathRemains()
        {
            _path.GetFullPath(Arg.Any<string>()).Throws(new Exception());
            var path = "inputPATH";
            Assert.DoesNotThrow(() =>
            {
                path = _scriptAction.ComposeScriptPath(path, new TokenReplacer());
            });
            Assert.AreEqual("inputPATH", path, "Path did not remain");
        }

        [Test]
        public void ApplyTokens_PathGetFullPathThrowsExpection_DoNotThrowExcpetion_PathRemains()
        {
            _path.GetFullPath(Arg.Any<string>()).Throws(new Exception());
            var job = new Job(new JobInfo(), _profile, _unusedAccounts);
            job.Profile.Scripting.ScriptFile = "inputPATH";

            Assert.DoesNotThrow(() =>
            {
                _scriptAction.ApplyPreSpecifiedTokens(job);
            });

            Assert.AreEqual("inputPATH", job.Profile.Scripting.ScriptFile, "Path did not remain");
        }
    }
}
