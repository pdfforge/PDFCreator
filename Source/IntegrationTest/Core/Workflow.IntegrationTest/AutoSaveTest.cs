using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Workflow
{
    [TestFixture]
    internal class AutoSaveTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("AutoSaveTest");
            _th.Profile.ShowProgress = false;

            _factory = container.GetInstance<ConversionWorkflowTestFactory>();
        }

        [TearDown]
        public void CleanUp()
        {
            _th?.CleanUp();
        }

        private TestHelper _th;
        private PdfCreatorSettings _settings;
        private ConversionWorkflowTestFactory _factory;

        private ConversionWorkflow BuildConversionWorkflow()
        {
            return _factory.BuildWorkflow();
        }

        private void CreateFile(string filename)
        {
            File.Create(filename).Dispose();
        }

        [Test]
        public void TestAutoSave_MultipleOutputFiles_EnsureUniqueFilenameIsDisabled_OutputFilenamesDoExist_ExistingFilesGetOverwritten()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Jpeg;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var expectedOutputFile1 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1.jpg");
            var expectedOutputFile2 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2.jpg");
            var expectedOutputFile3 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3.jpg");

            CreateFile(expectedOutputFile1);
            CreateFile(expectedOutputFile2);
            CreateFile(expectedOutputFile3);

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile1), "Expected first outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile2), "Expected second outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile3), "Expected third outputfile does not exist.");

            var fileInfo = new FileInfo(expectedOutputFile1);
            Assert.IsFalse(fileInfo.Length == 0, "Expected first outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile2);
            Assert.IsFalse(fileInfo.Length == 0, "Expected second outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile3);
            Assert.IsFalse(fileInfo.Length == 0, "Expected third outputfile is empty.");
        }

        [Test]
        public void TestAutoSave_MultipleOutputFiles_EnsureUniqueFilenameIsDisabled_SecondOutputFilenamesDoesExist_SecondExistingFileGetsOverwritten()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Jpeg;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var expectedOutputFile1 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1.jpg");
            var expectedOutputFile2 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2.jpg");
            var expectedOutputFile3 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3.jpg");

            CreateFile(expectedOutputFile2);

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile1), "Expected first outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile2), "Expected second outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile3), "Expected third outputfile does not exist.");

            var fileInfo = new FileInfo(expectedOutputFile1);
            Assert.IsFalse(fileInfo.Length == 0, "Expected first outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile2);
            Assert.IsFalse(fileInfo.Length == 0, "Expected second outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile3);
            Assert.IsFalse(fileInfo.Length == 0, "Expected third outputfile is empty.");
        }

        [Test]
        public void TestAutoSave_MultipleOutputFiles_EnsureUniqueFilenameIsEnabled_OutputFilenamesDoExist_OutputfilesGetAppendix()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = true;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Jpeg;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var existingFile1 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1.jpg");
            var existingFile2 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2.jpg");
            var existingFile3 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3.jpg");

            CreateFile(existingFile1);
            CreateFile(existingFile2);
            CreateFile(existingFile3);

            var expectedOutputFile1WithAppendix = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1_2.jpg");
            var expectedOutputFile2WithAppendix = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2_2.jpg");
            var expectedOutputFile3WithAppendix = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3_2.jpg");

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(existingFile1), "First existing file does not exist anymore.");
            var fileInfo = new FileInfo(existingFile1);
            Assert.IsTrue(fileInfo.Length == 0, "First existing file has been overwritten.");

            Assert.IsTrue(File.Exists(existingFile2), "Second existing file does not exist anymore.");
            fileInfo = new FileInfo(existingFile2);
            Assert.IsTrue(fileInfo.Length == 0, "Second existing file has been overwritten.");

            Assert.IsTrue(File.Exists(existingFile3), "Third existing file does not exist anymore.");
            fileInfo = new FileInfo(existingFile3);
            Assert.IsTrue(fileInfo.Length == 0, "Third existing file has been overwritten.");

            Assert.IsTrue(File.Exists(expectedOutputFile1WithAppendix), "Expected first outputfile (with appendix) does not exist.");
            fileInfo = new FileInfo(expectedOutputFile1WithAppendix);
            Assert.IsFalse(fileInfo.Length == 0, "Expected first outputfile (with appendix) is empty.");

            Assert.IsTrue(File.Exists(expectedOutputFile2WithAppendix), "Expected second outputfile (with appendix) does not exist.");
            fileInfo = new FileInfo(expectedOutputFile2WithAppendix);
            Assert.IsFalse(fileInfo.Length == 0, "Expected second outputfile (with appendix) is empty.");

            Assert.IsTrue(File.Exists(expectedOutputFile3WithAppendix), "Expected third outputfile (with appendix) does not exist.");
            fileInfo = new FileInfo(expectedOutputFile3WithAppendix);
            Assert.IsFalse(fileInfo.Length == 0, "Expected third outputfile (with appendix) is empty.");
        }

        [Test]
        public void TestAutoSave_MultipleOutputFiles_EnsureUniqueFilenameIsEnabled_SecondOutputFilenameDoesExist_SecondOutputfileGetsAppendix()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = true;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Jpeg;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var existingFile2 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2.jpg");

            CreateFile(existingFile2);

            var expectedOutputFile1 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1.jpg");
            var expectedOutputFile2WithAppendix = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2_2.jpg");
            var expectedOutputFile3 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3.jpg");

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(existingFile2), "Existing second file does not exist anymore.");
            var fileInfo = new FileInfo(existingFile2);
            Assert.IsTrue(fileInfo.Length == 0, "Existing second file has been overwritten.");

            Assert.IsTrue(File.Exists(expectedOutputFile1), "Expected first outputfile does not exist.");
            fileInfo = new FileInfo(expectedOutputFile1);
            Assert.IsFalse(fileInfo.Length == 0, "Expected first outputfile is empty.");

            Assert.IsTrue(File.Exists(expectedOutputFile2WithAppendix), "Expected second outputfile (with appendix) does not exist.");
            fileInfo = new FileInfo(expectedOutputFile2WithAppendix);
            Assert.IsFalse(fileInfo.Length == 0, "Expected second outputfile (with appendix) is empty.");

            Assert.IsTrue(File.Exists(expectedOutputFile3), "Expected third outputfile does not exist.");
            fileInfo = new FileInfo(expectedOutputFile3);
            Assert.IsFalse(fileInfo.Length == 0, "Expected third outputfile is empty.");
        }

        [Test]
        public void TestAutoSave_MultipleOutputFiles_EnsureUniqueFilenamesIsDisabled_OutputFilenamesDoNotExist()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Jpeg;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var expectedOutputFile1 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1.jpg");
            var expectedOutputFile2 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2.jpg");
            var expectedOutputFile3 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3.jpg");

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile1), "Expected first outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile2), "Expected second outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile3), "Expected third outputfile does not exist.");

            var fileInfo = new FileInfo(expectedOutputFile1);
            Assert.IsFalse(fileInfo.Length == 0, "Expected first outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile2);
            Assert.IsFalse(fileInfo.Length == 0, "Expected second outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile3);
            Assert.IsFalse(fileInfo.Length == 0, "Expected third outputfile is empty.");
        }

        [Test]
        public void TestAutoSave_MultipleOutputFiles_EnsureUniqueFilenamesIsEnabled_OutputFilenamesDoNotExist()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = true;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Jpeg;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var expectedOutputFile1 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput1.jpg");
            var expectedOutputFile2 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput2.jpg");
            var expectedOutputFile3 = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput3.jpg");

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile1), "Expected first outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile2), "Expected second outputfile does not exist.");
            Assert.IsTrue(File.Exists(expectedOutputFile3), "Expected third outputfile does not exist.");

            var fileInfo = new FileInfo(expectedOutputFile1);
            Assert.IsFalse(fileInfo.Length == 0, "Expected first outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile2);
            Assert.IsFalse(fileInfo.Length == 0, "Expected second outputfile is empty.");

            fileInfo = new FileInfo(expectedOutputFile3);
            Assert.IsFalse(fileInfo.Length == 0, "Expected third outputfile is empty.");
        }

        [Test]
        public void TestAutoSave_SingleOutputFile_EnsureUniqueFilenameIsDisabled_OutputFilenameDoesExist_ExistingFileGetsOverwritten()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;

            //create empty file, that should be overwritten
            var expectedOutputFile = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput.pdf");
            CreateFile(expectedOutputFile);

            var autoSaveWorkflow = BuildConversionWorkflow();

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile), "Expected outputfile does not exist.");

            var fileInfo = new FileInfo(expectedOutputFile);
            Assert.IsFalse(fileInfo.Length == 0, "Expected outputfile did not overwritte existing (empty) file.");
        }

        [Test]
        public void TestAutoSave_SingleOutputFile_EnsureUniqueFilenameIsEnabled_OutputFilenameDoesExist_OutputfileGetsAppendix()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = true;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;

            //create empty file with original output filename
            var existingFile = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput.pdf");
            CreateFile(existingFile);

            var expectedOutputFileWithAppendix = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput_2.pdf");

            var autoSaveWorkflow = BuildConversionWorkflow();

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(existingFile), "Already existing file with outputfilename does not exist anymore.");
            var fileInfo = new FileInfo(existingFile);
            Assert.IsTrue(fileInfo.Length == 0, "Already existing (empty) file with outputfilename has been overwritten.");

            Assert.IsTrue(File.Exists(expectedOutputFileWithAppendix), "Expected outputfile (with appendix) does not exist.");
            fileInfo = new FileInfo(expectedOutputFileWithAppendix);
            Assert.IsFalse(fileInfo.Length == 0, "Expected outputfile (with appendix) is empty.");
        }

        [Test]
        public void TestAutoSave_SingleOutputFile_EnsureUniqueFilenamesIsDisabled_OutputFilenameDoesNotExist()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var expectedOutputFile = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput.pdf");

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile), "Expected outputfile does not exist.");
            var fileInfo = new FileInfo(expectedOutputFile);
            Assert.IsFalse(fileInfo.Length == 0, "Expected outputfile is empty.");
        }

        [Test]
        public void TestAutoSave_SingleOutputFile_EnsureUniqueFilenamesIsEnabled_OutputFilenameDoesNotExist()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.AutoSave.Enabled = true;

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = true;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;

            var autoSaveWorkflow = BuildConversionWorkflow();

            var expectedOutputFile = Path.Combine(_th.TmpTestFolder, "AutoSaveTestOutput.pdf");

            autoSaveWorkflow.RunWorkflow(_th.Job);

            Assert.IsTrue(File.Exists(expectedOutputFile), "Expected outputfile does not exist.");
            var fileInfo = new FileInfo(expectedOutputFile);
            Assert.IsFalse(fileInfo.Length == 0, "Expected outputfile is empty.");
        }
    }
}
