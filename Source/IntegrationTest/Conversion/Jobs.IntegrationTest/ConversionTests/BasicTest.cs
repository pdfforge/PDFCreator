using iTextSharp.text.pdf;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    [TestFixture]
    public class BasicTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("BasicTest");
        }

        [TearDown]
        public void CleanUp()
        {
            if (_th != null)
                _th.CleanUp();
        }

        private TestHelper _th;

        private void CheckOutputFiles()
        {
            foreach (var file in _th.Job.OutputFiles)
            {
                Assert.IsTrue(File.Exists(file), "Output file '" + file + "' does not exist!");
                var fi = new FileInfo(file);
                Assert.IsTrue(fi.Length > 0, "Output file '" + file + "' is empty!");
            }
        }

        private void GenerateTestHelperWithManySourceFiles(int numberOfSourceFiles)
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            var sourceFile = _th.JobInfo.SourceFiles[0];

            // one file is in there already
            for (var i = 0; i < numberOfSourceFiles - 1; i++)
            {
                _th.JobInfo.SourceFiles.Add(sourceFile);
            }

            var jobInfoReader = new JobInfoManager(null, null);
            jobInfoReader.SaveToInfFile(_th.JobInfo);
        }

        [Test]
        public void Test_DefaultPdfVersionIs9_14()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.RunGsJob();

            CheckOutputFiles();

            using (var pdfReader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                Assert.AreEqual('4', pdfReader.PdfVersion);
            }
        }

        /// <summary>
        ///     From the MSDN documentation:
        ///     On Windows Vista and earlier versions of the Windows operating system, the length of the
        ///     arguments added to thelength of the full path to the process must be less than 2080. On
        ///     Windows 7 and later versions, the length must be less than 32699.
        /// </summary>
        [Test]
        public void TestManyPagesBy400PsFiles_toPdf()
        {
            // 400 is my tradeoff to create a fileset that is most likely to be large enough to
            // reproduce the error on most machines (if it would not be fixed). (42103 characters on my machine)
            const int numberOfSourceFiles = 400;

            GenerateTestHelperWithManySourceFiles(numberOfSourceFiles);

            _th.RunGsJob();

            CheckOutputFiles();

            using (var pdf = new PdfReader(_th.Job.OutputFiles[0]))
            {
                Assert.AreEqual(numberOfSourceFiles, pdf.NumberOfPages, "Number of output pages is incorrect");
            }
        }

        [Test]
        public void TestManyPagesFromOnePsFile_toPdf()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Pdf);

            _th.RunGsJob();

            CheckOutputFiles();

            using (var pdf = new PdfReader(_th.Job.OutputFiles[0]))
            {
                Assert.AreEqual(11, pdf.NumberOfPages, "Number of output pages is incorrect");
            }
        }

        [Test]
        public void TestManyPagesJpeg()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Jpeg);

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(11, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestManyPagesPng()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Png);

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(11, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestManyPagesTif()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Tif);

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestSinglePageJpeg()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Jpeg);

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestSinglePagePdf()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            CheckOutputFiles();

            using (var pdf = new PdfReader(_th.Job.OutputFiles[0]))
            {
                Assert.AreEqual(1, pdf.NumberOfPages, "Number of output pages is incorrect");
            }
        }

        [Test]
        public void TestSinglePagePng()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Png);

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestSinglePageTif()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Tif);

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestSpecialCharactersJpeg()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Jpeg);
            _th.SetFilenameTemplate("Täßt#Filênám€ 他们她们它们 вѣдѣ вѣди.jpg");
            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(11, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }

        [Test]
        public void TestSpecialCharactersPdf()
        {
            _th.GenerateGsJob(PSfiles.ElevenTextPages, OutputFormat.Pdf);

            _th.SetFilenameTemplate("Täßt#Filênám€ 他们她们它们 вѣдѣ вѣди.pdf");

            _th.RunGsJob();

            CheckOutputFiles();

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Number of output pages is incorrect");
        }
    }
}
