using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public static bool UseSodaPdf { get; set; }

        private string _cachedInstallationPath;
        private bool _wasSearched;

        private readonly IFile _file;
        private readonly IAssemblyHelper _assemblyHelper;

        // Tuple format: Item1: DisplayName in Registry, Item2: name of the exe file that has to exist in the InstallLocation
        private readonly Tuple<string, string>[] _currentPdfArchitectVersions =
        {
            new Tuple<string, string>("PDF Architect 9 Business", "architect-business.exe"),
            new Tuple<string, string>("PDF Architect 9", "architect.exe")
        };

        private readonly Tuple<string, string>[] _historicPdfArchitectCandidates =
        {
            new Tuple<string, string>("PDF Architect 8", "architect.exe"),
            new Tuple<string, string>("PDF Architect 7", "architect.exe"),
            new Tuple<string, string>("PDF Architect 6", "architect.exe"),
            new Tuple<string, string>("PDF Architect 5", "architect.exe"),
            new Tuple<string, string>("PDF Architect 4", "architect.exe"),
            new Tuple<string, string>("PDF Architect 3", "PDF Architect 3.exe"),
            new Tuple<string, string>("PDF Architect 3", "architect.exe"),
            new Tuple<string, string>("PDF Architect 2", "PDF Architect 2.exe"),
            new Tuple<string, string>("PDF Architect", "PDF Architect.exe")
        };

        private readonly Tuple<string, string>[] _sodaPdfCandidates =
        {
            new Tuple<string, string>("Soda PDF Desktop 14", "soda.exe")
        };

        private readonly IRegistry _registry;

        private readonly string[] _softwareKeys =
        {
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        public PdfArchitectCheck(IRegistry registry, IFile file, IAssemblyHelper assemblyHelper, IThreadManager threadManager)
        {
            _registry = registry;
            _file = file;
            _assemblyHelper = assemblyHelper;
            threadManager.StandbyEnded += OnStandbyEnded;
        }

        private void OnStandbyEnded(object sender, EventArgs args)
        {
            if (IsInstalled())
                return;

            // PDF Architect might be installed now
            Task.Run(() =>
            {
                _cachedInstallationPath = DoGetInstallationPath(_currentPdfArchitectVersions.Concat(_historicPdfArchitectCandidates).ToArray());
                _wasSearched = true;
            });
        }

        /// <summary>
        ///     Finds the exe path of a known version of PDF Architect by checking if the Registry contains a
        ///     programm with Publisher "pdfforge" and a known DisplayName
        /// </summary>
        /// <returns>Returns installation path if a known version of PDF Architect is installed, else null</returns>
        public string GetInstallationPath()
        {
            if (_wasSearched)
            {
                if (string.IsNullOrEmpty(_cachedInstallationPath))
                    _cachedInstallationPath = DoGetInstallationPath(_currentPdfArchitectVersions);

                return _cachedInstallationPath;
            }

            _cachedInstallationPath = DoGetInstallationPath(_currentPdfArchitectVersions.Concat(_historicPdfArchitectCandidates).ToArray());
            _wasSearched = true;

            return _cachedInstallationPath;
        }

        private string DoGetInstallationPath(Tuple<string, string>[] candidates)
        {
            var publisher = UseSodaPdf ? "Avanquest" : "pdfforge";

            foreach (var pdfArchitectCandidate in candidates)
            {
                try
                {
                    var installationPath = TryFindInstallationPath(pdfArchitectCandidate.Item1, pdfArchitectCandidate.Item2, publisher);

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
            try
            {
                var architectDirectory = PathSafe.Combine(_assemblyHelper.GetAssemblyDirectory(), "PDF Architect");
                var allFiles = Directory.GetFiles(architectDirectory);
                foreach (var file in allFiles)
                {
                    if (file.Contains("Installer") && file.EndsWith(".exe"))
                        return PathSafe.Combine(architectDirectory, file);
                }
            }
            catch
            { }

            return "";
        }

        private string TryFindInstallationPath(string msiDisplayName, string applicationExeName, string publisherName)
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
                                    sk.GetValue("Publisher").ToString().Contains(publisherName) &&
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
