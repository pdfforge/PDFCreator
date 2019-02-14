using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using pdfforge.PDFCreator.Core.UsageStatistics;
using SystemInterface;

namespace UsageStatistics.UnitTest
{
    [TestFixture]
    public class MachineIdGeneratorTest
    {
        private long _volumeSerial;
        private IKernel32 _kernel32;
        private IEnvironment _environment;

        [SetUp]
        public void SetUp()
        {
            _volumeSerial = 1;
            _environment = Substitute.For<IEnvironment>();
            _kernel32 = Substitute.For<IKernel32>();
            _kernel32.GetSystemVolumeSerial().Returns(x => _volumeSerial);
        }

        private MachineIdGenerator BuildGenerator()
        {
            return new MachineIdGenerator(_environment, _kernel32);
        }

        [Test]
        public void GetMachineId_ReturnedStringNotEmpty()
        {
            var machineIdGenerator = BuildGenerator();
            var machineId = machineIdGenerator.GetMachineId();

            Assert.IsNotEmpty(machineId);
        }

        [Test]
        public void GetMachineId_WithChangedVolumeSerial_ReturnsDifferentMachineIdHash()
        {
            var machineIdGenerator = BuildGenerator();
            var machineId = machineIdGenerator.GetMachineId();
            _volumeSerial = 2;

            var machineId2 = machineIdGenerator.GetMachineId();

            Assert.AreNotEqual(machineId, machineId2);
        }
    }
}
