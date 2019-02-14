using iTextSharp.text.pdf;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    internal class PdfMetadataTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("MetadataTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        [Test]
        public void TestMetadata()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.Job.JobInfo.Metadata.Title = "Title with äüößéèê€";
            _th.Job.JobInfo.Metadata.Subject = "Subject @";
            _th.Job.JobInfo.Metadata.Keywords = "Key1 Key2 Key3";
            _th.Job.JobInfo.Metadata.Author = "Author";

            _th.RunGsJob();

            using (var pdf = new PdfReader(_th.Job.OutputFiles[0]))
            {
                Assert.AreEqual(_th.Job.JobInfo.Metadata.Title, pdf.Info["Title"], "Wrong Title in Metadata");
                Assert.AreEqual(_th.Job.JobInfo.Metadata.Subject, pdf.Info["Subject"], "Wrong Subject in Metadata");
                Assert.AreEqual(_th.Job.JobInfo.Metadata.Keywords, pdf.Info["Keywords"], "Wrong Keywords in Metadata");
                Assert.AreEqual(_th.Job.JobInfo.Metadata.Author, pdf.Info["Author"], "Wrong Author in Metadata");
                Assert.AreEqual(_th.Job.Producer, pdf.Info["Producer"], "Wrong Producer in Metadata");
                Assert.AreEqual(_th.Job.Producer, pdf.Info["Creator"], "Wrong Creator in Metadata");
            }
        }
    }
}
