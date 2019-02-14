using System;
using System.IO;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IPdfArchitectCheck
    {
        /// <summary>
        ///     Finds the exe path of a known version of PDF Architect by checking if the Registry contains a
        ///     programm with Publisher "pdfforge" and a known DisplayName
        /// </summary>
        /// <returns>Returns installation path if a known version of PDF Architect is installed, else null</returns>
        string GetInstallationPath();

        /// <summary>
        ///     Check if a known version of PDF Architect is installed
        /// </summary>
        /// <returns>Returns true, if PDF Architect is installed</returns>
        bool IsInstalled();

        bool IsDownloaded();

        string GetInstallerPath();
    }

    public class PdfArchitectCheck : IPdfArchitectCheck
    {
        private string _cachedInstallationPath;
        private bool _wasSearched;

        private readonly IFile _file;
        private readonly IAssemblyHelper _assemblyHelper;

        // Tuple format: Item1: DisplayName in Registry, Item2: name of the exe file that has to exist in the InstallLocation
        private readonly Tuple<string, string>[] _pdfArchitectCandidates =
        {
            new Tuple<string, string>("PDF Architect 6", "architect.exe"),
            new Tuple<string, string>("PDF Architect 5", "architect.exe"),
            new Tuple<string, string>("PDF Architect 4", "architect.exe"),
            new Tuple<string, string>("PDF Architect 3", "PDF Architect 3.exe"),
            new Tuple<string, string>("PDF Architect 3", "architect.exe"),
            new Tuple<string, string>("PDF Architect 2", "PDF Architect 2.exe"),
            new Tuple<string, string>("PDF Architect", "PDF Architect.exe")
        };

        private readonly IRegistry _registry;

        private readonly string[] _softwareKeys =
        {
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        public PdfArchitectCheck(IRegistry registry, IFile file, IAssemblyHelper assemblyHelper)
        {
            _registry = registry;
            _file = file;
            _assemblyHelper = assemblyHelper;
        }

        /// <summary>
        ///     Finds the exe path of a known version of PDF Architect by checking if the Registry contains a
        ///     programm with Publisher "pdfforge" and a known DisplayName
        /// </summary>
        /// <returns>Returns installation path if a known version of PDF Architect is installed, else null</returns>
        public string GetInstallationPath()
        {
            if (_wasSearched)
                return _cachedInstallationPath;

            _cachedInstallationPath = DoGetInstallationPath();
            _wasSearched = true;

            return _cachedInstallationPath;
        }

        private string DoGetInstallationPath()
        {
            foreach (var pdfArchitectCandidate in _pdfArchitectCandidates)
            {
                try
                {
                    var installationPath = TryFindInstallationPath(pdfArchitectCandidate.Item1, pdfArchitectCandidate.Item2);

                    if (installationPath != null)
                        return installationPath;
                }
                catch (IOException)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///     Check if a known version of PDF Architect is installed
        /// </summary>
        /// <returns>Returns true, if PDF Architect is installed</returns>
        public bool IsInstalled()
        {
            return GetInstallationPath() != null;
        }

        public bool IsDownloaded()
        {
            var path = GetInstallerPath();
            return _file.Exists(path) || IsInstalled();
        }

        public string GetInstallerPath()
        {
            return Path.Combine(_assemblyHelper.GetAssemblyDirectory(), "PDF Architect", "architect-setup.exe");
        }

        private string TryFindInstallationPath(string msiDisplayName, string applicationExeName)
        {
            foreach (var key in _softwareKeys)
            {
                using (var rk = _registry.LocalMachine.OpenSubKey(key))
                {
                    if (rk == null)
                        continue;

                    //Let's go through the registry keys and get the info we need:
                    foreach (var skName in rk.GetSubKeyNames())
                    {
                        try
                        {
                            using (var sk = rk.OpenSubKey(skName))
                            {
                                var displayNameKey = sk?.GetValue("DisplayName");
                                if (displayNameKey == null)
                                    continue;

                                //If the key has value, continue, if not, skip it:
                                var displayName = displayNameKey.ToString();
                                if (displayName.StartsWith(msiDisplayName, StringComparison.OrdinalIgnoreCase) &&
                                    !displayName.Contains("Enterprise") &&
                                    sk.GetValue("Publisher").ToString().Contains("pdfforge") &&
                                    (sk.GetValue("InstallLocation") != null))
                                {
                                    var installLocation = sk.GetValue("InstallLocation").ToString();
                                    var exePath = Path.Combine(installLocation, applicationExeName);

                                    if (_file.Exists(exePath))
                                        return exePath;

                                    // if the exe does not exist, this is the wrong path
                                    return null;
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
            return null;
        }
    }
}
