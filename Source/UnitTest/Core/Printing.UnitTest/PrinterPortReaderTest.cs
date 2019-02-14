using NUnit.Framework;
using pdfforge.PDFCreator.Core.Printing.Port;
using Rhino.Mocks;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.UnitTest.Core.Printing
{
    [TestFixture]
    public class PrinterPortReaderTest
    {
        [SetUp]
        public void SetUp()
        {
            _hklmKey = MockRepository.GenerateStub<IRegistryKey>();

            _registry = MockRepository.GenerateStub<IRegistry>();
            _registry.Stub(x => x.LocalMachine).Return(_hklmKey);

            _printerPortReader = new PrinterPortReader(_registry);
        }

        private IRegistry _registry;
        private IRegistryKey _hklmKey;
        private PrinterPortReader _printerPortReader;

        private IRegistryKey CreateWorkingPortRegistryKey(string description = "Random Description", string program = @"C:\RandomApplication.exe", string type = "PS", string tempFolderName = "PDFCreator", bool? server = false, int? jobCounter = 0)
        {
            var key = MockRepository.GenerateStub<IRegistryKey>();

            key.Stub(x => x.GetValue("Description")).Return(description);
            key.Stub(x => x.GetValue("Program")).Return(program);
            key.Stub(x => x.GetValue("Type", "PS")).Return(type);
            key.Stub(x => x.GetValue("TempFolderName", "PDFCreator")).Return(tempFolderName);
            key.Stub(x => x.GetValue("JobCounter", 0)).Return(jobCounter);

            if (server == null)
                key.Stub(x => x.GetValue("Server")).Return(null);
            else
                key.Stub(x => x.GetValue("Server")).Return((bool)server ? 1 : 0);

            _hklmKey.Stub(x => x.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Print\Monitors\pdfcmon\Ports\pdfcmon"))
                .Return(key);

            return key;
        }

        [Test]
        public void ReadFromRegistry_IfJobCounterDoesNotExists_ReturnsZero()
        {
            CreateWorkingPortRegistryKey();
            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");
            Assert.AreEqual(0, printerPort.JobCounter);
        }

        [Test]
        public void ReadFromRegistry_ReturnsGivenJobCounter()
        {
            CreateWorkingPortRegistryKey(jobCounter: 3);
            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");
            Assert.AreEqual(3, printerPort.JobCounter);
        }

        [Test]
        public void ReadFromRegistry_ReturnsGivenPortNameName()
        {
            CreateWorkingPortRegistryKey(tempFolderName: " ");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual("pdfcmon", printerPort.Name);
        }

        [Test]
        public void ReadFromRegistry_WithCompleteRegistrySetup_ClosesRegistryKey()
        {
            var key = CreateWorkingPortRegistryKey();

            _printerPortReader.ReadPrinterPort("pdfcmon");

            key.AssertWasCalled(x => x.Close());
        }

        [Test]
        public void ReadFromRegistry_WithCompleteRegistrySetup_ReturnsPrinterPort()
        {
            CreateWorkingPortRegistryKey();

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsNotNull(printerPort);
        }

        [Test]
        public void ReadFromRegistry_WithInvalidPortType_ReturnsTypePs()
        {
            CreateWorkingPortRegistryKey(type: "Xyz");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual(PortType.PostScript, printerPort.Type);
        }

        [Test]
        public void ReadFromRegistry_WithMissingDescription_ReturnsNull()
        {
            CreateWorkingPortRegistryKey(null);

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsNull(printerPort);
        }

        [Test]
        public void ReadFromRegistry_WithMissingProgram_ReturnsNull()
        {
            CreateWorkingPortRegistryKey(program: null);

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsNull(printerPort);
        }

        [Test]
        public void ReadFromRegistry_WithNoRegistrySetup_ReturnsNull()
        {
            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsNull(printerPort);
        }

        [Test]
        public void ReadFromRegistry_WithoutServerEntry_ReturnsStandardPort()
        {
            CreateWorkingPortRegistryKey(server: null);

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsFalse(printerPort.IsServerPort);
        }

        [Test]
        public void ReadFromRegistry_WithoutServerFlag_ReturnsStandardPort()
        {
            CreateWorkingPortRegistryKey(server: false);

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsFalse(printerPort.IsServerPort);
        }

        [Test]
        public void ReadFromRegistry_WithServerFlag_ReturnsServerPort()
        {
            CreateWorkingPortRegistryKey(server: true);

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.IsTrue(printerPort.IsServerPort);
        }

        [Test]
        public void ReadFromRegistry_WithSpecificDescription_ReturnsDescription()
        {
            CreateWorkingPortRegistryKey("My specific description");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual("My specific description", printerPort.Description);
        }

        [Test]
        public void ReadFromRegistry_WithSpecificProgram_ReturnsProgram()
        {
            CreateWorkingPortRegistryKey(program: @"C:\Program Files\MyApp.exe");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual(@"C:\Program Files\MyApp.exe", printerPort.Program);
        }

        [Test]
        public void ReadFromRegistry_WithSpecificTempFolderName_ReturnsTempFolderName()
        {
            CreateWorkingPortRegistryKey(tempFolderName: "Xyz");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual("Xyz", printerPort.TempFolderName);
        }

        [Test]
        public void ReadFromRegistry_WithWhitespaceAsTempFolderName_ReturnsDefaultTempFolderName()
        {
            CreateWorkingPortRegistryKey(tempFolderName: " ");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual("PDFCreator", printerPort.TempFolderName);
        }

        [Test]
        public void ReadFromRegistry_WithXpsPort_ReturnsTypeXps()
        {
            CreateWorkingPortRegistryKey(type: "XPS");

            var printerPort = _printerPortReader.ReadPrinterPort("pdfcmon");

            Assert.AreEqual(PortType.Xps, printerPort.Type);
        }
    }
}
