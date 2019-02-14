using CustomScriptActionIntegrationTest;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.CustomScriptAction;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.IO;
using SystemWrapper.IO;

namespace CustomScriptAction.IntegrationTest
{
    [TestFixture]
    public class CustomScriptHandlerTest
    {
        private CustomScriptHandler _customScriptHandler;
        private ConversionProfile _profile;
        private JobInfo _jobInfo;
        private Job _job;
        private CustomScriptTestEnvironment _testEnvironment;

        [SetUp]
        public void SetUp()
        {
            _testEnvironment = CustomScriptTestEnvironment.Init(TestScript.SetFilenameInPreConversionCreateBackUpInPostConversion);

            _customScriptHandler = new CustomScriptHandler(new CsScriptLoader(new FileWrap(), _testEnvironment.AssemblyHelper));

            _profile = new ConversionProfile();
            _jobInfo = new JobInfo();
            _job = new Job(_jobInfo, _profile, new Accounts());
            _job.Profile.CustomScript.Enabled = true;
            _job.Profile.CustomScript.ScriptFilename = _testEnvironment.ScriptFilename;
        }

        [TearDown]
        public void TearDown()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void ExecuteScriptPreConversion_SampleScriptSetsFileNameTemplate()
        {
            _job.OutputFileTemplate = "RandomTemplate";

            var result = _customScriptHandler.ExecutePreConversion(_job);

            Assert.IsTrue(result, "ActionResult");
            var expectedOutputFilenameTemplate = "Filename from Script";
            Assert.AreEqual(expectedOutputFilenameTemplate, _job.OutputFileTemplate);
        }

        [Test]
        public void ExecuteScriptPostConversion_SampleScriptCreatesBackUp()
        {
            var tempFileName = "SomeOutputFile.txt";
            var tempFile = TempFileHelper.CreateTempFile("CsScriptTest", tempFileName, "File Content");
            _job.OutputFiles.Add(tempFile);

            var result = _customScriptHandler.ExecutePostConversion(_job);

            Assert.IsTrue(result, "ActionResult");
            var expectedBackUpFile = Path.Combine(tempFile + "_BackUp", tempFileName);
            Assert.IsTrue(File.Exists(expectedBackUpFile), "Did not create BackUp File");
        }
    }
}
