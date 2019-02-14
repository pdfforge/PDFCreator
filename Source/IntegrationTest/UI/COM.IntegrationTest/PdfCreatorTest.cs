using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Editions.PDFCreator;
using pdfforge.PDFCreator.UI.COM;
using SimpleInjector;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    internal class PdfCreatorTest
    {
        [OneTimeSetUp]
        public void CleanDependencies()
        {
            ComDependencyBuilder.ResetDependencies();
            ComTestHelper.ModifyAndBuildComDependencies();
        }

        [SetUp]
        public void SetUp()
        {
            LoggingHelper.InitConsoleLogger("PDFCreatorTest", LoggingLevel.Off);

            _queue = new Queue();
            _queue.Initialize();

            _pdfCreatorObj = new PdfCreatorObj();

            var bootstrapper = new PDFCreatorBootstrapper();
            var container = new Container();
            bootstrapper.RegisterMainApplication(container);
            _th = container.GetInstance<TestHelper>();
        }

        [TearDown]
        public void TearDown()
        {
            _queue.ReleaseCom();
        }

        private PdfCreatorObj _pdfCreatorObj;
        private Queue _queue;
        private TestHelper _th;

        [Test]
        public void WithEmptyStringAsArgument_ThrowComException()
        {
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(string.Empty));
            StringAssert.Contains("The specified path must not be empty or uninitiliazed.", ex.Message);
        }

        [Test]
        public void WithInvalidFileExtensionAsArgument_ThrowComException()
        {
            _th.InitTempFolder("CmdFileTest"); //Important here: Neither a .ps file nor a .pdf file
            var path = _th.GenerateTestFile(TestFile.ScriptCopyFilesToDirectoryCMD);
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(path));

            _th.CleanUp();
            StringAssert.Contains("Only .ps and .pdf files can be directly added to the queue.", ex.Message);
        }

        [Test]
        public void WithNonExistingFile_ThrowComException()
        {
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue("asdasdasdasd"));
            StringAssert.Contains("File with such a path doesn't exist. Please check if the specified path is correct.", ex.Message);
        }

        [Test]
        public void WithNullStringAsArgument_ThrowComException()
        {
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(null));
            StringAssert.Contains("The specified path must not be empty or uninitiliazed.", ex.Message);
        }

        [Test]
        public void WithOtherInvalidFileExtensionAsArgument_ThrowComException()
        {
            _th.InitTempFolder("CertFileTest");
            var filePath = _th.GenerateTestFile(TestFile.CertificationFileP12);
            var ex = Assert.Throws<COMException>(() => _pdfCreatorObj.AddFileToQueue(filePath));

            _th.CleanUp();
            StringAssert.Contains("Only .ps and .pdf files can be directly added to the queue.", ex.Message);
        }

        [Test]
        public void WithPdfFileAsArgument_AddItToQueue()
        {
            var dependencies = ComTestHelper.ModifyAndBuildComDependencies();

            _th.InitTempFolder("PDFTest");
            var path = _th.GenerateTestFile(TestFile.PDFCreatorTestpage_GS9_19_PDF); //The pdf file content is irrelevant for this test.
            var queueInstance = dependencies.QueueAdapter.JobInfoQueue;
            var jobNumber = queueInstance.Count;

            _pdfCreatorObj.AddFileToQueue(path);

            Assert.AreEqual(++jobNumber, queueInstance.Count);
            queueInstance.Remove(queueInstance.JobInfos[--jobNumber], true);
            _th.CleanUp();
        }

        [Test]
        public void WithPsFileAsArgument_AddItToQueue()
        {
            var dependencies = ComTestHelper.ModifyAndBuildComDependencies();

            _th.InitTempFolder("PsTest");
            var path = _th.GenerateTestFile(TestFile.PDFCreatorTestpagePs);
            var queueInstance = dependencies.QueueAdapter.JobInfoQueue;
            var jobNumber = queueInstance.Count;

            _pdfCreatorObj.AddFileToQueue(path);

            Assert.AreEqual(++jobNumber, queueInstance.Count);
            queueInstance.Remove(queueInstance.JobInfos[--jobNumber], true);
            _th.CleanUp();
        }
    }
}
