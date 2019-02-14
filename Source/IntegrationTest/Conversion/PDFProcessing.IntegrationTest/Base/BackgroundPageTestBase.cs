using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Diagnostics;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    internal abstract class BackgroundPageTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;
        private PdfReader _reader;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        [SetUp]
        public void SetUp()
        {
            _pdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder($"PDFProcessing_{_pdfProcessor.GetType().Name}_Background");

            _th.Profile.BackgroundPage.File = _th.GenerateTestFile(TestFile.Background3PagesPDF);
            _th.Profile.BackgroundPage.Enabled = true;

            _th.Profile.CoverPage.File = _th.GenerateTestFile(TestFile.Cover2PagesPDF);
            _th.Profile.AttachmentPage.File = _th.GenerateTestFile(TestFile.Attachment3PagesPDF);
        }

        [TearDown]
        public void CleanUp()
        {
            _reader?.Close();
            _th.CleanUp();
            FinalizePdfProcessor();
        }

        [Test]
        public void NoRepetitionPlusPdfVersionTest()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");

            PdfVersionTester.CheckPDFVersion(_reader, _th.Job.Profile, _pdfProcessor);
        }

        [Test]
        public void NoRepetitionAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //document
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void NoRepetitionAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //document
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void NoRepetitionCoverWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background page on 1. cover page");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background page on 2. cover page");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4);
            Assert.AreEqual(" ", pageText, "2. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5);
            Assert.AreEqual(" ", pageText, "3. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
        }

        [Test]
        public void NoRepetitionCoverWithBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background page on 1. cover page");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background page on 2. cover page");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4);
            Assert.AreEqual(" ", pageText, "2. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5);
            Assert.AreEqual(" ", pageText, "3. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void NoRepetitionCoverWithBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background page on 1. cover page");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background page on 2. cover page");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4);
            Assert.AreEqual(" ", pageText, "2. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5);
            Assert.AreEqual(" ", pageText, "3. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void NoRepetitionCoverWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "1. page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
        }

        [Test]
        public void NoRepetitionCoverWithoutBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "1. page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void NoRepetitionCoverWithoutBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "1. page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6);
            Assert.AreEqual(" ", pageText, "4. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7);
            Assert.AreEqual(" ", pageText, "5. document page is not empty");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8);
            Assert.AreEqual(" ", pageText, "6. document page is not empty");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void RepeatAllPages()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
        }

        [Test]
        public void RepeatAllPagesAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //document
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
        }

        [Test]
        public void RepeatAllPagesAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //document
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void RepeatAllPagesCoverWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 6. page of document.");
        }

        [Test]
        public void RepeatAllPagesCoverWithBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 3. attachment page.");
        }

        [Test]
        public void RepeatAllPagesCoverWithBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void RepeatAllPagesCoverWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
        }

        [Test]
        public void RepeatAllPagesCoverWithoutBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
        }

        [Test]
        public void RepeatAllPagesCoverWithoutBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void RepeatLastPage()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
        }

        [Test]
        public void RepeatLastPageAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //document
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
        }

        [Test]
        public void RepeatLastPageAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = false;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //document
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void RepeatLastPageCoverWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
        }

        [Test]
        public void RepeatLastPageCoverWithBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
        }

        [Test]
        public void RepeatLastPageCoverWithBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test]
        public void RepeatLastPageCoverWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = false;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
        }

        [Test]
        public void RepeatLastPageCoverWithoutBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
        }

        [Test]
        public void RepeatLastPageCoverWithoutBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);

            _reader = new PdfReader(_th.Job.OutputFiles[0]);
            //Cover
            var pageText = PdfTextExtractor.GetTextFromPage(_reader, 1).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 2).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
            //Document
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 3).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 4).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 5).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 6).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 7).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 8).Replace("\n", "").Replace(" ", "");
            Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            //attachment
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 9).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 10).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
            pageText = PdfTextExtractor.GetTextFromPage(_reader, 11).Replace("\n", "").Replace(" ", "");
            Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
            Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
        }

        [Test, Explicit("This has to be checked manually")]
        public void OnlyForManualTesting()
        {
            if (!Debugger.IsAttached)
                return;

            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;
            _th.Profile.BackgroundPage.File = _th.GenerateTestFile(TestFile.PortraitLandscapeLandscapeLandscapePortraitPDF);
            _th.GenerateGsJob_WithSetOutput(TestFile.SixEmptyPagesPDF);

            _pdfProcessor.ProcessPdf(_th.Job);
            Process.Start(_th.Job.OutputFiles[0]);
            Debugger.Break();
        }
    }
}
