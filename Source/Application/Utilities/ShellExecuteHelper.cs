using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Utilities
{
    public enum ShellExecuteResult
    {
        Success,
        Failed,
        RunAsWasDenied
    }

    public class ShellExecuteHelper : IShellExecuteHelper
    {
        public ShellExecuteResult RunAsAdmin(string path, string arguments)
        {
            var psi = new ProcessStartInfo();

            psi.FileName = path;
            psi.Arguments = arguments;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            // Use ShellExecute and RunAs starting with Windows Vista
            if (Environment.OSVersion.Version.Major >= 6)
            {
                psi.UseShellExecute = true;
                psi.Verb = "runas";
            }

            try
            {
                var process = System.Diagnostics.Process.Start(psi);

                if (process == null)
                    return ShellExecuteResult.Failed;

                process.WaitForExit(30000);

                return process.ExitCode == 0
                    ? ShellExecuteResult.Success : ShellExecuteResult.Failed;
            }
            catch (Win32Exception)
            {
                return ShellExecuteResult.RunAsWasDenied;
            }
            catch (SystemException)
            {
                return ShellExecuteResult.Failed;
            }
        }

        public async Task<ShellExecuteResult> RunAsAdminAsync(string path, string arguments)
        {
            var psi = new ProcessStartInfo();

            psi.FileName = path;
            psi.Arguments = arguments;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            // Use ShellExecute and RunAs starting with Windows Vista
            if (Environment.OSVersion.Version.Major >= 6)
            {
                psi.UseShellExecute = true;
                psi.Verb = "runas";
            }

            try
            {
                var tcs = new TaskCompletionSource<int>();
                var process = new System.Diagnostics.Process();
                process.StartInfo = psi;
                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) => tcs.SetResult(process.ExitCode);

                process.Start();

                var exitCode = await tcs.Task;

                return exitCode == 0
                    ? ShellExecuteResult.Success : ShellExecuteResult.Failed;
            }
            catch (Win32Exception)
            {
                return ShellExecuteResult.RunAsWasDenied;
            }
            catch (SystemException)
            {
                return ShellExecuteResult.Failed;
            }
        }
    }
}
