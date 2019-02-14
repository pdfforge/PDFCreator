using System.Text.RegularExpressions;
using SystemInterface;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IMachineIdGenerator
    {
        string GetMachineId();
    }

    public class MachineIdGenerator : IMachineIdGenerator
    {
        private const int MaxMachineNameLength = 50;
        private readonly IEnvironment _environment;
        private readonly IKernel32 _kernel32;

        public MachineIdGenerator(IEnvironment environment, IKernel32 kernel32)
        {
            _environment = environment;
            _kernel32 = kernel32;
        }

        public string GetMachineId()
        {
            var machineName = GetSanitizedMachineName();
            var volumeSerial = _kernel32.GetSystemVolumeSerial().ToString("X8");
            var machineHash = MachineId.GetSha1Hash(machineName + volumeSerial);

            return $"{machineHash}";
        }

        private string GetSanitizedMachineName()
        {
            var machineName = "";
            if (_environment?.MachineName != null)
            {
                machineName = _environment.MachineName.ToUpperInvariant();
                machineName = Regex.Replace(machineName, @"[^0-9^A-Z^\-_]", "_");
            }

            return machineName.Length < MaxMachineNameLength
                    ? machineName
                    : machineName.Substring(0, 50);
        }
    }
}
