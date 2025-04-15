using System.Diagnostics;
using SystemInterface.Diagnostics;
using SystemWrapper.Diagnostics;

namespace pdfforge.PDFCreator.Utilities.Process
{
    public interface IProcessStarter
    {
        IProcess Start(string fileName);

        IProcess Start(string fileName, bool useShellExecute);

        IProcess Start(string fileName, string arguments);

        bool Start(IProcessStartInfo startInfo);

        IProcess CreateProcess(string fileName);
        IProcess StartWithSameElevation(ProcessStartInfo startInfo);

    }

    public class ProcessStarter : IProcessStarter
    {
        public IProcess Start(string fileName)
        {
            var process = new ProcessWrap();
            process.Start(fileName);

            return process;
        }

        public IProcess Start(string fileName, bool useShellExecute)
        {
            var process = new ProcessWrap();

            var startInfo = new ProcessStartInfoWrap
            {
                FileName = fileName,
                UseShellExecute = useShellExecute
            };

            process.StartInfo = startInfo;
            process.Start();

            return process;
        }

        public IProcess Start(string fileName, string arguments)
        {
            var process = new ProcessWrap();
            process.Start(fileName, arguments);

            return process;
        }

        public bool Start(IProcessStartInfo startInfo)
        {
            var process = new ProcessWrap();
            process.StartInfo = startInfo;
            return process.Start();
        }

        public IProcess StartWithSameElevation(ProcessStartInfo startInfo)
        {
            var process = new ProcessWrap();
            process.StartInfo = new ProcessStartInfoWrap(startInfo);
            process.Start();
            return process;
        }

        public IProcess CreateProcess(string fileName)
        {
            var process = new ProcessWrap();
            process.StartInfo.FileName = fileName;

            return process;
        }
    }
}
