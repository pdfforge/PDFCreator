using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Printing.PrintFile
{
    [TestFixture]
    internal class PrintCommandTest
    {
        private IPrinterHelper _printerHelper;
        private int _timeOut;

        [SetUp]
        public void SetUp()
        {
            _printerHelper = new PrinterHelper(new SystemPrinterProvider());
            _timeOut = 60;
        }

        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void PrintCommand_GivenFileWithUnknownExtension_IsUnprintable()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.unknownExtension");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            Assert.IsFalse(printCommand.IsPrintable);
            Assert.AreEqual(PrintType.Unprintable, printCommand.CommandType);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_AllowsPrint()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            Assert.IsTrue(printCommand.CommandType == PrintType.Print);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_HasCorrectStartInfo()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");
            var printCommand = new PrintCommand(tempFile, _printerHelper.GetDefaultPrinter(), new FileAssoc(), _printerHelper, _timeOut);

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            StringAssert.Contains(tempFile, factory.LastMock.StartInfo.Arguments);
        }

        [Test]
        public void PrintCommand_GivenValidPrintOnlyFile_PrintsSuccessfully()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");
            var printCommand = new PrintCommand(tempFile, _printerHelper.GetDefaultPrinter(), new FileAssoc(), _printerHelper, _timeOut);

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.IsFalse(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_AllowsPrintTo()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            Assert.IsTrue(printCommand.CommandType == PrintType.PrintTo);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_HasCorrectStartInfo()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");
            const string printer = "SomePrinter";

            var printCommand = new PrintCommand(tempFile, printer, new FileAssoc(), _printerHelper, _timeOut);

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            StringAssert.Contains("\"" + printer + "\"", factory.LastMock.StartInfo.Arguments);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_IsPrintable()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            Assert.IsTrue(printCommand.IsPrintable);
        }

        [Test]
        public void PrintCommand_GivenValidTextFile_PrintsSuccessfully()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            var factory = new MockProcessWrapperFactory(true);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsTrue(printCommand.Successful);
            Assert.IsFalse(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintCommand_OnPrintingNonExistingFile_ThrowsException()
        {
            var printCommand = new PrintCommand("NotExistingFile", "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            Assert.Throws<InvalidOperationException>(() => printCommand.Print());
        }

        [Test]
        public void PrintCommand_WithNonExistingFile_IsNotPrintable()
        {
            var printCommand = new PrintCommand("NotExistingFile", "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            Assert.IsFalse(printCommand.IsPrintable);
        }

        [Test]
        public void PrintCommand_WithProcessNotFinishing_GetsKilled()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.txt");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            var factory = new MockProcessWrapperFactory(false);
            printCommand.ProcessWrapperFactory = factory;

            printCommand.Print();

            Assert.IsFalse(printCommand.Successful);
            Assert.IsTrue(factory.LastMock.WasKilled);
        }

        [Test]
        public void PrintOnlyFile_GivenWithoutSettingsDefaultPrinter_ThrowsException()
        {
            var tempFile = TempFileHelper.CreateTempFile("PrintCommand", "test.ini");

            var printCommand = new PrintCommand(tempFile, "SomePrinter", new FileAssoc(), _printerHelper, _timeOut);

            printCommand.ProcessWrapperFactory = new MockProcessWrapperFactory(true);

            Assert.Throws<InvalidOperationException>(() => printCommand.Print());
        }

        [Test]
        public void FileHasJpgAsExtensionType_PrintReturnsTrue()
        {
            var specialCommandArguments = Environment.SystemDirectory + "\\shimgvw.dll,imageview_printto";
            var testFile = TempFileHelper.CreateTempFile("PicturePrintFallbackTest", "test.jpg");
            var fileAssoc = new FileAssoc();
            var printCommand = new PrintCommand(testFile, "SomePrinter", fileAssoc, _printerHelper, _timeOut);

            var fileIsPrintable = printCommand.Print();
            var shellCommand = fileAssoc.GetShellCommand(Path.GetExtension(testFile), "printto");

            Assert.AreEqual(specialCommandArguments.ToUpper(), shellCommand.Arguments[0].ToUpper());
            Assert.IsTrue(fileIsPrintable);
        }

        [Test]
        public void FileHasPngAsExtensionType_PrintReturnsTrue()
        {
            var specialCommandArguments = Environment.SystemDirectory + "\\shimgvw.dll,imageview_printto";
            var testFile = TempFileHelper.CreateTempFile("PicturePrintFallbackTest", "test.png");
            var fileAssoc = new FileAssoc();
            var printCommand = new PrintCommand(testFile, "SomePrinter", fileAssoc, _printerHelper, _timeOut);

            var fileIsPrintable = printCommand.Print();
            var shellCommand = fileAssoc.GetShellCommand(Path.GetExtension(testFile), "printto");

            Assert.AreEqual(specialCommandArguments.ToUpper(), shellCommand.Arguments[0].ToUpper());
            Assert.IsTrue(fileIsPrintable);
        }

        [Test]
        public void FileHasTiffAsExtensionType_PrintReturnsTrue()
        {
            var specialCommandArguments = Environment.SystemDirectory + "\\shimgvw.dll,imageview_printto";
            var testFile = TempFileHelper.CreateTempFile("PicturePrintFallbackTest", "test.tiff");
            var fileAssoc = new FileAssoc();
            var printCommand = new PrintCommand(testFile, "SomePrinter", fileAssoc, _printerHelper, _timeOut);

            var fileIsPrintable = printCommand.Print();
            var shellCommand = fileAssoc.GetShellCommand(Path.GetExtension(testFile), "printto");

            Assert.AreEqual(specialCommandArguments.ToUpper(), shellCommand.Arguments[0].ToUpper());
            Assert.IsTrue(fileIsPrintable);
        }
    }
}
