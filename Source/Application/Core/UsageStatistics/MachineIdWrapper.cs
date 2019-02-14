using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IMachineId
    {
        string GetMachineId();
    }

    internal class MachineIdWrapper : IMachineId
    {
        private readonly IKernel32 _kernel32;
        private readonly IRegistry _registry;

        public MachineIdWrapper(IRegistry registry, IKernel32 kernel32)
        {
            _registry = registry;
            _kernel32 = kernel32;
        }

        public string GetMachineId()
        {
            var systemVolumeSerial = _kernel32.GetSystemVolumeSerial();
            var windowsProductId = GetWindowsProductId();
            var id = new MachinId(systemVolumeSerial, windowsProductId);
            return id.CalculateMachineHash();
        }

        private string GetWindowsProductId()
        {
            var v = _registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductId", null);
            if (v == null)
                return "";
            return v.ToString();
        }
    }
}
