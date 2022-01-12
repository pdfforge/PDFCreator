using CliWrap;
using Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using System;
using System.IO;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.CLI.Helper
{
    public interface IApplicationLauncher
    {
        Task<int> LaunchApplication(params string[] args);

        bool CanLaunchApplication();
    }

    public class ApplicationLauncher : IApplicationLauncher
    {
        private readonly string _exeName;
        private readonly IInstallationPathProvider _installationPathProvider;

        public ApplicationLauncher(string exeName, IInstallationPathProvider installationPathProvider)
        {
            _exeName = exeName;
            _installationPathProvider = installationPathProvider;
        }

        public async Task<int> LaunchApplication(params string[] args)
        {
            var path = GetPDFCreatorPath();

            var call = Cli.Wrap(path)
                .WithArguments(args)
                .WithWorkingDirectory(Path.GetDirectoryName(path))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .WithValidation(CommandResultValidation.None);

            Console.WriteLine($"Calling \"{path}\" {call.Arguments}");

            var result = await call.ExecuteAsync();

            return result.ExitCode;
        }

        public bool CanLaunchApplication()
        {
            try
            {
                GetPDFCreatorPath();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetPDFCreatorPath()
        {
            var assemblyPath = Path.GetDirectoryName(Path.GetFullPath(GetType().Assembly.Location));
            var path = Path.Combine(assemblyPath, _exeName);

            if (File.Exists(path))
                return path;

            path = GetPathFromRegistry();
            if (File.Exists(path))
                return path;

            throw new Exception($"Could not find {_exeName}!");
        }

        private string GetPathFromRegistry()
        {
            var regPath = Registry.GetValue(PathSafe.Combine("HKEY_LOCAL_MACHINE", _installationPathProvider.ApplicationRegistryPath, "Program"), "ApplicationPath", "");
            return Path.Combine((string)regPath, _exeName);
        }
    }
}
