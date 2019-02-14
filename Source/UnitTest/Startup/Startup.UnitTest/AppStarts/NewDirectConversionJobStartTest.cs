using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.Workflow;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UnitTest.Startup.AppStarts
{
    internal class DirectConversionStartTestClass : DirectConversionStart
    {
        public DirectConversionStartTestClass(IDirectConversionInfFileHelper directConversionInfFileHelper)
            : this(Substitute.For<IJobInfoQueue>(), Substitute.For<IMaybePipedApplicationStarter>(), Substitute.For<IJobInfoManager>(), directConversionInfFileHelper)
        { }

        public DirectConversionStartTestClass(IJobInfoQueue jobInfoQueue, IMaybePipedApplicationStarter maybePipedApplicationStarter, IJobInfoManager jobInfoManager, IDirectConversionInfFileHelper directConversionInfFileHelper)
            : base(jobInfoQueue, maybePipedApplicationStarter, jobInfoManager, directConversionInfFileHelper)
        { }

        public string PublicComposePipeMessage()
        {
            return ComposePipeMessage();
        }

        public bool PublicStartApplication()
        {
            return StartApplication();
        }
    }

    [TestFixture]
    public class NewDirectConversionJobStartTest
    {
        private DirectConversionStartTestClass _directConversionStart;
        private IDirectConversionInfFileHelper _directConversionInfFileHelper;

        [SetUp]
        public void SetUp()
        {
            _directConversionInfFileHelper = Substitute.For<IDirectConversionInfFileHelper>();
            _directConversionStart = new DirectConversionStartTestClass(_directConversionInfFileHelper);
        }

        [Test]
        public void PublicComposePipeMessage_SingleDirectConversionFile_NoMerge_ReturnsMessageWithNewInfFile()
        {
            _directConversionStart.AppStartParameters.Merge = false;
            _directConversionStart.DirectConversionFiles.Add("somefile.pdf");
            _directConversionInfFileHelper.TransformToInfFile("somefile.pdf", Arg.Any<AppStartParameters>()).Returns("someNewInfFile");

            var pipeMessage = _directConversionStart.PublicComposePipeMessage();

            Assert.AreEqual("NewJob|someNewInfFile", pipeMessage);
        }

        [Test]
        public void PublicComposePipeMessage_SingleDirectConversionFile_MergeEnabled_ReturnsMessageWithNewInfFile()
        {
            _directConversionStart.AppStartParameters.Merge = true;
            _directConversionStart.DirectConversionFiles.Add("somefile.pdf");
            _directConversionInfFileHelper.TransformToInfFileWithMerge(_directConversionStart.DirectConversionFiles, Arg.Any<AppStartParameters>()).Returns("someNewInfFile");

            var pipeMessage = _directConversionStart.PublicComposePipeMessage();

            Assert.AreEqual("NewJob|someNewInfFile", pipeMessage);
        }

        [Test]
        public void PublicComposePipeMessage_MultipleDirectConversionFiles_MergeEnabled_ReturnsMessageWithNewInfFile()
        {
            _directConversionStart.AppStartParameters.Merge = false;
            _directConversionStart.DirectConversionFiles.Add("somefile1.pdf");
            _directConversionInfFileHelper.TransformToInfFile("somefile1.pdf", Arg.Any<AppStartParameters>()).Returns("someNewInfFile1.inf");
            _directConversionStart.DirectConversionFiles.Add("somefile2.pdf");
            _directConversionInfFileHelper.TransformToInfFile("somefile2.pdf", Arg.Any<AppStartParameters>()).Returns("someNewInfFile2.inf");

            var pipeMessage = _directConversionStart.PublicComposePipeMessage();

            Assert.AreEqual("NewJob|someNewInfFile1.inf|someNewInfFile2.inf", pipeMessage);
        }

        [Test]
        public void PublicComposePipeMessage_MergeIsEnabledCallTransformToInfFile_ReturnsMessageWithSingleNewInfFiles()
        {
            _directConversionStart.AppStartParameters.Merge = true;
            _directConversionStart.DirectConversionFiles.Add("somefile.pdf");
            _directConversionStart.DirectConversionFiles.Add("somefile.pdf");
            _directConversionInfFileHelper.TransformToInfFileWithMerge(Arg.Any<List<string>>(), Arg.Any<AppStartParameters>()).Returns("someNewInfFile");

            var pipeMessage = _directConversionStart.PublicComposePipeMessage();

            Assert.AreEqual("NewJob|someNewInfFile", pipeMessage);
        }

        [Test]
        public void NewDirectConversionJobStart_TransformToInfFile_StartApplicationReturnsTrue()
        {
            _directConversionStart.DirectConversionFiles.Add("someFile.ps");
            _directConversionInfFileHelper.TransformToInfFile("someFile.ps", Arg.Any<AppStartParameters>()).Returns("someNewInfFile");

            var resultOfStartApplication = _directConversionStart.PublicStartApplication();

            Assert.IsTrue(resultOfStartApplication);
        }

        [Test]
        public void NewDirectConversionJobStart_TransformToInfFileReturnsEmptyString_StartApplicationReturnsFalse()
        {
            _directConversionStart.DirectConversionFiles.Add("someFile.ps");
            _directConversionInfFileHelper.TransformToInfFile("someFile.ps", Arg.Any<AppStartParameters>()).Returns("");

            var resultOfStartApplication = _directConversionStart.PublicStartApplication();

            Assert.IsFalse(resultOfStartApplication);
        }
    }
}
