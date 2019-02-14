using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    public class AppSettingsCheckerTest
    {
        private AppSettingsChecker _appSettingsChecker;
        private IDefaultViewerCheck _defaultViewerCheck;
        private PdfCreatorSettings _pdfCreatorSettings;
        private DefaultViewer _defaultViewer;
        private IFile _file;

        [SetUp]
        public void SetUp()
        {
            _defaultViewerCheck = Substitute.For<IDefaultViewerCheck>();
            _file = Substitute.For<IFile>();
            _appSettingsChecker = new AppSettingsChecker(_defaultViewerCheck);
            var storage = Substitute.For<IStorage>();
            _pdfCreatorSettings = new PdfCreatorSettings();
            _defaultViewer = new DefaultViewer();
            _defaultViewer.IsActive = true;
            _defaultViewer.Path = "Some Path";
            _pdfCreatorSettings.DefaultViewers.Add(_defaultViewer);
        }

        [Test]
        public void DefaultViewerWithoutErrors_ReturnsTrue()
        {
            _defaultViewerCheck.Check(Arg.Any<DefaultViewer>()).Returns(new ActionResult());

            var result = _appSettingsChecker.CheckDefaultViewers(_pdfCreatorSettings.DefaultViewers);

            Assert.IsTrue(result);
        }

        [Test]
        public void DefaultViewerWithError_ReturnsError()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _defaultViewer.Path = "";
            _defaultViewerCheck.Check(_defaultViewer).Returns(new ActionResult(ErrorCode.DefaultViewer_Not_Found));

            var result = _appSettingsChecker.CheckDefaultViewers(_pdfCreatorSettings.DefaultViewers);

            Assert.Contains(ErrorCode.DefaultViewer_Not_Found, result, "Did not detect error.");
        }

        [Test]
        public void DefaultViewerWithMultipleErrors_ReturnsError()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _defaultViewer.Path = "";
            var checkResult = new ActionResult();
            checkResult.AddRange(new[] { ErrorCode.DefaultViewer_Not_Found, ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf });
            _defaultViewerCheck.Check(_defaultViewer).Returns(checkResult);

            var result = _appSettingsChecker.CheckDefaultViewers(_pdfCreatorSettings.DefaultViewers);

            Assert.Contains(ErrorCode.DefaultViewer_Not_Found, result, "Did not detect error.");
            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf, result, "Did not detect error.");
        }
    }
}
