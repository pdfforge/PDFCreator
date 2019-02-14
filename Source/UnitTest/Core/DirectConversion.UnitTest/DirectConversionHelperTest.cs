using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.DirectConversion.UnitTest
{
    [TestFixture]
    public class DirectConversionHelperTest
    {
        private IDirectConversionHelper _directConversionHelper;
        private IPdfProcessor _pdfProcessor;
        private IFile _file;

        [SetUp]
        public void SetUp()
        {
            _pdfProcessor = Substitute.For<IPdfProcessor>();
            _file = Substitute.For<IFile>();

            _directConversionHelper = new DirectConversionHelper(_pdfProcessor, _file);
        }

        [TestCase(@"C:\Test.pdf")]
        [TestCase(@"C:\Test.PDF")]
        [TestCase(@"C:\Test.ps")]
        [TestCase(@"C:\Test.PS")]
        [TestCase(@"C:\Some weird folder name\WithDot.doc\Test.PS")]
        public void CanConvertDirectly_WithAppropriateFilenames_IsTrue(string filename)
        {
            Assert.IsTrue(_directConversionHelper.CanConvertDirectly(filename));
        }

        [TestCase(@"C:\Test.doc")]
        [TestCase(@"C:\Test")]
        [TestCase(@"C:\Some weird folder name\WithDot.doc\Test")]
        public void CanConvertDirectly_WithInappropriateFilenames_IsFalse(string filename)
        {
            Assert.IsFalse(_directConversionHelper.CanConvertDirectly(filename));
        }

        [Test]
        public void GetNumberOfPages_InputIsPdfFile_ReturnsNumberOfPagesFromPdfProcessor()
        {
            var expectedNumberOfPages = 12345;
            var filename = "pdffile.pdf";
            _pdfProcessor.GetNumberOfPages(filename).Returns(expectedNumberOfPages);

            Assert.AreEqual(expectedNumberOfPages, _directConversionHelper.GetNumberOfPages(filename));
        }

        [Test]
        public void GetNumberOfPages_InputIsPsFile_DoesNotCallPdfProcessor()
        {
            var filename = "pdffile.ps";

            _directConversionHelper.GetNumberOfPages(filename);

            _pdfProcessor.DidNotReceive().GetNumberOfPages(Arg.Any<string>());
        }

        [Test]
        public void GetNumberOfPages_InputIsPsFile_FileOpenReadThrowsExcpetion_Returns1()
        {
            var filename = "pdffile.ps";
            _file.OpenRead(filename).Throws(new Exception());

            Assert.AreEqual(1, _directConversionHelper.GetNumberOfPages(filename));
        }
    }
}
