using NUnit.Framework;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.COM;
using System;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    internal class PrinterDevicesTest
    {
        [SetUp]
        public void SetUp()
        {
            // TODO add own initialization?
            _printerDevices = new Printers(new PrinterHelper(new SystemPrinterProvider()));
        }

        [TearDown]
        public void TearDown()
        {
        }

        private Printers _printerDevices;

        [Test]
        public void GetPrinterByIndex_IfIndexNegative_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _printerDevices.GetPrinterByIndex(-1));
        }

        [Test]
        public void GetPrinterByIndex_IfIndexTooBig_ThrowArgumentException()
        {
            var count = _printerDevices.Count;

            Assert.Throws<ArgumentException>(() => _printerDevices.GetPrinterByIndex(count + 1));
        }
    }
}
