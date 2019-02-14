using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    public class TempFolderProviderSpecialCharacters : ITempFolderProvider
    {
        public TempFolderProviderSpecialCharacters()
        {
            TempFolder = TempFileHelper.CreateTempFolder("PdfCreatorTest\\" + "TempFolder for " + TestHelper.SpecialCharactersString);
        }

        public string TempFolder { get; }
    }

    public class TempFolderProvider : ITempFolderProvider
    {
        public TempFolderProvider()
        {
            TempFolder = TempFileHelper.CreateTempFolder("PdfCreatorTest\\" + "SpecialCharactersTest");
        }

        public string TempFolder { get; }
    }

    [TestFixture]
    internal class SpecialCharactersTest
    {
        [SetUp]
        public void SetUp()
        {
            _tempFolderStack = new Stack<string>();
            _tempFolderStack.Push(Path.GetTempPath());
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
            ResetTempFolder();
        }

        private TestHelper _th;
        private Stack<string> _tempFolderStack;

        private void SetUpTestWithSpecialCharactersInTempFolder()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();

            container.Options.AllowOverridingRegistrations = true;
            container.Register<ITempFolderProvider, TempFolderProviderSpecialCharacters>();
            container.Options.AllowOverridingRegistrations = false;

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SpecialCharactersTest");
        }

        private void SetUpTestWithRegularTempFolder()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();

            container.Options.AllowOverridingRegistrations = true;
            container.Register<ITempFolderProvider, TempFolderProvider>();
            container.Options.AllowOverridingRegistrations = false;

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SpecialCharactersTest");
        }

        private void CheckOutputFiles()
        {
            Assert.IsTrue(_th.Job.OutputFiles.Count > 0, "Did not create output files.");

            foreach (var file in _th.Job.OutputFiles)
            {
                Assert.IsTrue(File.Exists(file), "Output file '" + file + "' does not exist!");
                var fi = new FileInfo(file);
                Assert.IsTrue(fi.Length > 0, "Output file '" + file + "' is empty!");
            }
        }

        [Test]
        public void SpecialCharacters_in_AttachmentFile()
        {
            SetUpTestWithRegularTempFolder();

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            var attachmentFile = _th.GenerateTestFile(TestFile.Attachment3PagesPDF);
            var attachmentFileWithSpecialCharacters =
                Path.Combine(Path.GetDirectoryName(attachmentFile),
                    Path.GetFileNameWithoutExtension(attachmentFile)
                    + TestHelper.SpecialCharactersString
                    + Path.GetExtension(attachmentFile));
            File.Move(attachmentFile, attachmentFileWithSpecialCharacters);

            _th.Job.Profile.AttachmentPage.Enabled = true;
            _th.Job.Profile.AttachmentPage.File = attachmentFileWithSpecialCharacters;

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_CoverFile()
        {
            SetUpTestWithRegularTempFolder();

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            var coverFile = _th.GenerateTestFile(TestFile.Cover2PagesPDF);
            var coverFileWithSpecialCharacters =
                Path.Combine(Path.GetDirectoryName(coverFile),
                    Path.GetFileNameWithoutExtension(coverFile)
                    + TestHelper.SpecialCharactersString
                    + Path.GetExtension(coverFile));
            File.Move(coverFile, coverFileWithSpecialCharacters);

            _th.Job.Profile.CoverPage.Enabled = true;
            _th.Job.Profile.CoverPage.File = coverFileWithSpecialCharacters;

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_EnvironmentTempFolder_Pdf()
        {
            SetUpTestWithRegularTempFolder();

            var newTemp = Path.Combine(Path.GetTempPath(), TestHelper.SpecialCharactersString);
            SetTempFolder(newTemp);

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.RunGsJob();
            CheckOutputFiles();
        }

        //other formats

        [Test]
        public void SpecialCharacters_in_OutputFilename()
        {
            SetUpTestWithRegularTempFolder();

            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Pdf);

            _th.SetFilenameTemplate(TestHelper.SpecialCharactersString + ".pdf");

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_SourceFile()
        {
            SetUpTestWithRegularTempFolder();

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            var sourceFileWithSpecialCharacters =
                Path.Combine(Path.GetDirectoryName(_th.Job.JobInfo.SourceFiles[0].Filename),
                    Path.GetFileNameWithoutExtension(_th.Job.JobInfo.SourceFiles[0].Filename)
                    + TestHelper.SpecialCharactersString
                    + Path.GetExtension(_th.Job.JobInfo.SourceFiles[0].Filename));

            File.Move(_th.Job.JobInfo.SourceFiles[0].Filename, sourceFileWithSpecialCharacters);
            _th.Job.JobInfo.SourceFiles[0].Filename = sourceFileWithSpecialCharacters;
            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_Pdf()
        {
            SetUpTestWithSpecialCharactersInTempFolder();
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_PdfA1B()
        {
            SetUpTestWithSpecialCharactersInTempFolder();
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfA1B);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_PdfA2B()
        {
            SetUpTestWithSpecialCharactersInTempFolder();
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfA2B);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_in_TempFolder_PdfX()
        {
            SetUpTestWithSpecialCharactersInTempFolder();
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);

            _th.RunGsJob();

            CheckOutputFiles();
        }

        [Test]
        public void SpecialCharacters_Stamping()
        {
            SetUpTestWithRegularTempFolder();

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.Job.Profile.Stamping.Enabled = true;
            _th.Job.Profile.Stamping.StampText = TestHelper.SpecialCharactersString;

            _th.RunGsJob();

            CheckOutputFiles();
        }

        private void SetTempFolder(string path)
        {
            _tempFolderStack.Push(path);
            Directory.CreateDirectory(path);
            Environment.SetEnvironmentVariable("TEMP", path);
            Environment.SetEnvironmentVariable("TMP", path);
        }

        private void ResetTempFolder()
        {
            if (_tempFolderStack.Count <= 1)
                return;

            var folder = _tempFolderStack.Pop();

            SetTempFolder(_tempFolderStack.Peek());
            Directory.Delete(folder, true);
        }
    }
}
