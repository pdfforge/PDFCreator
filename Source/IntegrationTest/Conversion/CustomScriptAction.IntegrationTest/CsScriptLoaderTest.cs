using CSScriptLibrary;
using NSubstitute;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.CustomScriptAction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Utilities;
using System.IO;
using SystemInterface.IO;

namespace CustomScriptActionIntegrationTest
{
    [TestFixture]
    public class CsScriptLoaderTest
    {
        private CsScriptLoader _customScriptLoader;
        private IFile _file;
        private CustomScriptTestEnvironment _testEnvironment;

        [SetUp]
        public void SetUp()
        {
            _testEnvironment = CustomScriptTestEnvironment.Init(TestScript.SetFilenameInPreConversionCreateBackUpInPostConversion);

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);
            _customScriptLoader = new CsScriptLoader(_file, _testEnvironment.AssemblyHelper);
        }

        [TearDown]
        public void CleanUp()
        {
            _testEnvironment.CleanUp();
            TempFileHelper.CleanUp();
        }

        private string MockTempFolderAsAssemblyDirAndCreateScriptFile(string content)
        {
            var tempFolder = TempFileHelper.CreateTempFolder(nameof(CsScriptLoaderTest));
            var assemblyHelper = Substitute.For<IAssemblyHelper>();
            assemblyHelper.GetAssemblyDirectory().Returns(tempFolder);
            _customScriptLoader = new CsScriptLoader(_file, assemblyHelper);
            Directory.CreateDirectory(_customScriptLoader.ScriptFolder);
            var scriptFile = Path.Combine(_customScriptLoader.ScriptFolder, "TestScript.cs");
            File.WriteAllText(scriptFile, content);

            return scriptFile;
        }

        [Test]
        public void Init_CSScriptSearchDirs_ContainsAssemblyDir()
        {
            StringAssert.Contains(_testEnvironment.AssemblyDir, CSScript.GlobalSettings.SearchDirs);
        }

        [Test]
        public void Init_ScriptFolder_ReturnsExpectedFolder()
        {
            Assert.AreEqual(_testEnvironment.ExcpectedScriptFolder, _customScriptLoader.ScriptFolder);
        }

        [Test]
        public void Init_CSScriptSearchDirs_ContainsScriptFolder()
        {
            StringAssert.Contains(_testEnvironment.ExcpectedScriptFolder, CSScript.GlobalSettings.SearchDirs);
        }

        [Test]
        public void LoadScriptWithValidation_ScriptIsNotSet_ReturnsCorrespondingLoadResult()
        {
            var scriptFile = "";

            var loadResult = _customScriptLoader.LoadScriptWithValidation(scriptFile);

            var result = loadResult.Result;
            Assert.Contains(ErrorCode.CustomScript_NoScriptFileSpecified, result);
            result.Remove(ErrorCode.CustomScript_NoScriptFileSpecified);
            Assert.IsTrue(result, "Unexpected ErrorCodes in ActionResult");

            Assert.IsNull(loadResult.Script, "Script");

            Assert.IsEmpty(loadResult.ExceptionMessage, "ExceptionMessage");
        }

        [Test]
        public void LoadScriptWithValidation_ScriptFileInProgramDirDoesNotExist_ReturnsCorrespondingLoadResult()
        {
            _file.Exists(_testEnvironment.ScriptFile).Returns(false);

            var loadResult = _customScriptLoader.LoadScriptWithValidation(_testEnvironment.ScriptFile);

            var result = loadResult.Result;
            Assert.Contains(ErrorCode.CustomScript_FileDoesNotExistInScriptFolder, result);
            result.Remove(ErrorCode.CustomScript_FileDoesNotExistInScriptFolder);
            Assert.IsTrue(result, "Unexpected ErrorCodes in ActionResult");

            Assert.IsNull(loadResult.Script, "Script");

            Assert.IsEmpty(loadResult.ExceptionMessage, "ExceptionMessage");
        }

        [Test]
        public void LoadScriptWithValidation_EmptyScript_ReturnsCorrespondingLoadResult()
        {
            var emptyScript = MockTempFolderAsAssemblyDirAndCreateScriptFile("");

            var loadResult = _customScriptLoader.LoadScriptWithValidation(emptyScript);

            var result = loadResult.Result;

            Assert.Contains(ErrorCode.CustomScript_ErrorDuringCompilation, result);
            result.Remove(ErrorCode.CustomScript_ErrorDuringCompilation);
            Assert.IsTrue(result, "Unexpected ErrorCodes in ActionResult");

            Assert.IsNull(loadResult.Script, "Script");

            //We set the above ErrorCode while loadResult.ExceptionMessage remains empty.
            //This must not be tested, in case that a future CS-Sript Version throws an exception.
        }

        [Test]
        public void LoadScriptWithValidation_InvalidScript_ReturnsCorrespondingLoadResult()
        {
            var invalidScript = MockTempFolderAsAssemblyDirAndCreateScriptFile("Invalid Code");

            var loadResult = _customScriptLoader.LoadScriptWithValidation(invalidScript);

            var result = loadResult.Result;

            Assert.Contains(ErrorCode.CustomScript_ErrorDuringCompilation, result);
            result.Remove(ErrorCode.CustomScript_ErrorDuringCompilation);
            Assert.IsTrue(result, "Unexpected ErrorCodes in ActionResult");

            Assert.IsNull(loadResult.Script, "Script");

            Assert.IsNotEmpty(loadResult.ExceptionMessage, "ExceptionMessage");
        }

        [Test]
        public void LoadScriptWithValidation_ValidScript_ReturnsCorrespondingLoadResult()
        {
            var loadResult = _customScriptLoader.LoadScriptWithValidation(_testEnvironment.ScriptFile);

            Assert.IsEmpty(loadResult.ExceptionMessage, $"Unexpected Excecption: {loadResult.ExceptionMessage}");
            Assert.IsTrue(loadResult.Result, "Unexpected ErrorCodes in ActionResult");
            Assert.IsNotNull(loadResult.Script, "Script");
        }
    }
}
