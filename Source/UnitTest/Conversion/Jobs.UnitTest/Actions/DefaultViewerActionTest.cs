using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class DefaultViewerActionTest
    {
        private DefaultViewerAction _defaultViewerAction;
        private IFileAssoc _fileAssoc;
        private IRecommendArchitect _recommendArchitect;
        private IPdfArchitectCheck _pdfArchitectCheck;
        private ISettingsProvider _settingsProvider;
        private OutputFormatHelper _outputFormatHelper;
        private IProcessStarter _processStarter;
        private IDefaultViewerCheck _defaultViewerCheck;

        private ConversionProfile _profile;
        private PdfCreatorSettings _settings;
        private DefaultViewer _defaultViewer;
        private Job _job;
        private const string ArchitectPath = "Path\\PDF Architect";
        private const string DefaultViewerPath = @"X:\Apps\MyDefaultViewer.exe";

        [SetUp]
        public void SetUp()
        {
            _settings = new PdfCreatorSettings();
            _fileAssoc = Substitute.For<IFileAssoc>();
            _recommendArchitect = Substitute.For<IRecommendArchitect>();
            _pdfArchitectCheck = Substitute.For<IPdfArchitectCheck>();
            _pdfArchitectCheck.GetInstallationPath().Returns(ArchitectPath);
            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.Settings.Returns(_settings);
            _outputFormatHelper = new OutputFormatHelper();
            _processStarter = Substitute.For<IProcessStarter>();
            _defaultViewerCheck = Substitute.For<IDefaultViewerCheck>();
            _defaultViewer = new DefaultViewer();
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _defaultViewer.Path = DefaultViewerPath;
            _settings.DefaultViewers.Add(_defaultViewer);
            _job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());

            _profile = new ConversionProfile();

            _defaultViewerAction = new DefaultViewerAction(_fileAssoc, _recommendArchitect, _pdfArchitectCheck, _settingsProvider, _outputFormatHelper, _processStarter, _defaultViewerCheck);
        }

        [Test]
        public void IsEnabled_ReturnsProfileOpenViewer()
        {
            _profile.OpenViewer = true;
            Assert.IsTrue(_defaultViewerAction.IsEnabled(_profile));

            _profile.OpenViewer = false;
            Assert.IsFalse(_defaultViewerAction.IsEnabled(_profile));
        }

        [Test]
        public void OpenWithArchitect_StartsPDFArchitectProcessForEveryFileAndResturnsTrue()
        {
            var file = "file1";

            var result = _defaultViewerAction.OpenWithArchitect(file);

            Assert.IsTrue(result);
            _processStarter.Received().Start(ArchitectPath, "\"" + file + "\"");
        }

        [Test]
        public void OpenWithArchitect_ProcessStarterThrowsException_ResultContainsCorrespondingError()
        {
            _processStarter.Start(Arg.Any<string>(), Arg.Any<string>()).Throws(new Exception());

            var result = _defaultViewerAction.OpenWithArchitect("file");

            Assert.Contains(ErrorCode.Viewer_ArchitectCouldNotOpenOutput, result);
            result.Remove(ErrorCode.Viewer_ArchitectCouldNotOpenOutput);
            Assert.IsTrue(result, "Unexepcted results: " + result);
        }

        [Test]
        public void OpenOutputFile_NoDefaultViewer_FileAssocHasOpen_OpensOutputFile()
        {
            var filePath = @"X:\SomeFile\test.pdf";
            _fileAssoc.HasOpen(".pdf").Returns(true);

            var result = _defaultViewerAction.OpenOutputFile(filePath);

            Assert.IsTrue(result.IsSuccess);
            _processStarter.Received(1).Start(filePath);
        }

        [Test]
        public void OpenOutputFile_NoDefaultViewer_PdfHasNoOpen_RecommendsPdfArchitect()
        {
            var filePath = @"X:\SomeFile\test.pdf";
            _fileAssoc.HasOpen(".pdf").Returns(false);

            var result = _defaultViewerAction.OpenOutputFile(filePath);

            Assert.IsTrue(result.IsSuccess);
            _recommendArchitect.Received(1).Show();
        }

        [Test]
        public void OpenOutputFile_NoDefaultViewer_PngHasNoOpen_TriesToOpenAnyway()
        {
            var filePath = @"X:\SomeFile\test.png";
            _fileAssoc.HasOpen(".png").Returns(false);

            var result = _defaultViewerAction.OpenOutputFile(filePath);

            Assert.IsTrue(result.IsSuccess);
            _processStarter.Received(1).Start(filePath);
        }

        [Test]
        public void OpenOutputFile_NoDefaultViewer_AnExceptionIsThrown_ResultIsSuccess()
        {
            var filePath = @"X:\SomeFile\test.png";
            _fileAssoc.HasOpen(".png").Returns(false);
            _processStarter.Start(Arg.Any<string>()).Returns(x => throw new Exception());

            var result = _defaultViewerAction.OpenOutputFile(filePath);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void OpenOutputFile_WithDefaultViewer_FileAssocHasOpen_OpensOutputFile()
        {
            var filePath = @"X:\SomeFile\test.pdf";
            _fileAssoc.HasOpen(".pdf").Returns(true);
            _defaultViewer.IsActive = true;
            _defaultViewerCheck.Check(_defaultViewer).Returns(new ActionResult());

            var result = _defaultViewerAction.OpenOutputFile(filePath);

            Assert.IsTrue(result.IsSuccess);
            _processStarter.DidNotReceive().Start(filePath);
            _processStarter.Received(1).Start(_defaultViewer.Path, "\"" + filePath + "\"");
        }

        [Test]
        public void OpenOutputFile_WithDefaultViewer_DefaultViewerCheckFails_ResultIsError()
        {
            var filePath = @"X:\SomeFile\test.pdf";
            _fileAssoc.HasOpen(".pdf").Returns(true);
            _defaultViewer.IsActive = true;
            _defaultViewerCheck.Check(_defaultViewer).Returns(new ActionResult(ErrorCode.DefaultViewer_Not_Found));

            var result = _defaultViewerAction.OpenOutputFile(filePath);

            Assert.IsFalse(result.IsSuccess);
            Assert.Contains(ErrorCode.DefaultViewer_Not_Found, result);
            _processStarter.DidNotReceive().Start(filePath);
            _processStarter.DidNotReceive().Start(_defaultViewer.Path, "\"" + filePath + "\"");
        }

        [Test]
        public void ProcessJob_WithPngFile_OpensFile()
        {
            var filePath = @"X:\SomeFile\test.png";
            _job.OutputFiles.Add(filePath);
            _fileAssoc.HasOpen(".png").Returns(true);

            var result = _defaultViewerAction.ProcessJob(_job);

            Assert.IsTrue(result.IsSuccess);
            _processStarter.Received(1).Start(filePath);
        }

        [Test]
        public void ProcessJob_WithPdf_NoArchitect_OpensFile()
        {
            var filePath = @"X:\SomeFile\test.pdf";
            _job.OutputFiles.Add(filePath);
            _fileAssoc.HasOpen(".pdf").Returns(true);
            _pdfArchitectCheck.IsInstalled().Returns(false);

            var result = _defaultViewerAction.ProcessJob(_job);

            Assert.IsTrue(result.IsSuccess);
            _processStarter.Received(1).Start(filePath);
        }

        [Test]
        public void ProcessJob_WithPdf_WithArchitect_OpensArchitect()
        {
            var filePath = @"X:\SomeFile\test.pdf";
            _job.OutputFiles.Add(filePath);
            _fileAssoc.HasOpen(".pdf").Returns(true);
            _pdfArchitectCheck.IsInstalled().Returns(true);

            var result = _defaultViewerAction.ProcessJob(_job);

            Assert.IsTrue(result.IsSuccess);
            _processStarter.Received(1).Start(ArchitectPath, "\"" + filePath + "\"");
        }
    }
}
