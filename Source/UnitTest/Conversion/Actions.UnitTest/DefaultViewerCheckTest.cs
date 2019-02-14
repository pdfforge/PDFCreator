using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using SystemInterface.IO;

namespace Actions.UnitTest
{
    [TestFixture]
    public class DefaultViewerCheckTest
    {
        private DefaultViewerCheck _defaultViewerCheck;
        private DefaultViewer _defaultViewer;
        private IFile _file;

        [SetUp]
        public void SetUp()
        {
            _file = Substitute.For<IFile>();

            _defaultViewer = new DefaultViewer();
            _defaultViewer.IsActive = true;
            _defaultViewer.Path = "Some Path";

            _defaultViewerCheck = new DefaultViewerCheck(_file);
        }

        [Test]
        public void DefaultViewerIsNotActive_ReturnsSuccess()
        {
            _defaultViewer.IsActive = false;

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.IsTrue(result.IsSuccess);
        }

        #region Pdf

        [Test]
        public void PDF_DefaultViewerPathIsEmpty_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _defaultViewer.Path = "";

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_PathIsEmpty_for_Pdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void PDF_DefaultViewerFileDoesNotExist_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _file.Exists(_defaultViewer.Path).Returns(false);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_FileDoesNotExist_For_Pdf, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_FileDoesNotExist_For_Pdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void PDF_DefaultViewerFileExists_ReturnsTrue()
        {
            _defaultViewer.OutputFormat = OutputFormat.Pdf;
            _file.Exists(_defaultViewer.Path).Returns(true);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Pdf

        #region Jpeg

        [Test]
        public void Jpeg_DefaultViewerPathIsEmpty_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Jpeg;
            _defaultViewer.Path = "";

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Jpeg, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_PathIsEmpty_for_Jpeg);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Jpeg_DefaultViewerFileDoesNotExist_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Jpeg;
            _file.Exists(_defaultViewer.Path).Returns(false);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_FileDoesNotExist_For_Jpeg, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_FileDoesNotExist_For_Jpeg);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Jpeg_DefaultViewerFileExists_ReturnsTrue()
        {
            _defaultViewer.OutputFormat = OutputFormat.Jpeg;
            _file.Exists(_defaultViewer.Path).Returns(true);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Jpeg

        #region Png

        [Test]
        public void Png_DefaultViewerPathIsEmpty_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Png;
            _defaultViewer.Path = "";

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Png, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_PathIsEmpty_for_Png);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Png_DefaultViewerFileDoesNotExist_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Png;
            _file.Exists(_defaultViewer.Path).Returns(false);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_FileDoesNotExist_For_Png, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_FileDoesNotExist_For_Png);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Png_DefaultViewerFileExists_ReturnsTrue()
        {
            _defaultViewer.OutputFormat = OutputFormat.Png;
            _file.Exists(_defaultViewer.Path).Returns(true);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Png

        #region Tif

        [Test]
        public void Tif_DefaultViewerPathIsEmpty_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Tif;
            _defaultViewer.Path = "";

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Tif, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_PathIsEmpty_for_Tif);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Tif_DefaultViewerFileDoesNotExist_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Tif;
            _file.Exists(_defaultViewer.Path).Returns(false);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_FileDoesNotExist_For_Tif, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_FileDoesNotExist_For_Tif);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Tif_DefaultViewerFileExists_ReturnsTrue()
        {
            _defaultViewer.OutputFormat = OutputFormat.Tif;
            _file.Exists(_defaultViewer.Path).Returns(true);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Tif

        #region Txt

        [Test]
        public void Txt_DefaultViewerPathIsEmpty_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Txt;
            _defaultViewer.Path = "";

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_PathIsEmpty_for_Txt, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_PathIsEmpty_for_Txt);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Txt_DefaultViewerFileDoesNotExist_ReturnsErrorCode()
        {
            _defaultViewer.OutputFormat = OutputFormat.Txt;
            _file.Exists(_defaultViewer.Path).Returns(false);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.Contains(ErrorCode.DefaultViewer_FileDoesNotExist_For_Txt, result, "Did not detect error.");
            result.Remove(ErrorCode.DefaultViewer_FileDoesNotExist_For_Txt);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Txt_DefaultViewerFileExists_ReturnsTrue()
        {
            _defaultViewer.OutputFormat = OutputFormat.Txt;
            _file.Exists(_defaultViewer.Path).Returns(true);

            var result = _defaultViewerCheck.Check(_defaultViewer);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        #endregion Txt
    }
}
