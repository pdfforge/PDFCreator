using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Core.Services.Download;
using pdfforge.PDFCreator.UI.Presentation.Helper.Interfaces;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using SystemInterface;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.SetupDownloadHelper
{
    public class SetupDownloadHelper : ISetupDownloadHelper
    {
        private readonly IDownloader _downloader;
        private readonly IPath _path;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHashUtil _hashUtil;
        private readonly IProcessStarter _processStarter;
        private readonly IEnvironment _environment;

        public string DownloadedSetupPath = "";

        public SetupDownloadHelper(IDownloader downloader, IPath path, IFile file, IHashUtil hashUtil, IProcessStarter processStarter, IEnvironment environment)
        {
            _downloader = downloader;
            _path = path;
            _file = file;
            _hashUtil = hashUtil;
            _processStarter = processStarter;
            _environment = environment;
        }
        public async Task<bool> DownloadSetup(string updateInfoUrl, string sectionName, CancellationToken cancellationToken, Action<int> progress)
        {
            _logger.Info("Starting the setup download process.");
            var setupToDownload = await GetSetupInfo(updateInfoUrl, sectionName);
            if (setupToDownload.DownloadUrl == null || setupToDownload.FileHash == null)
            {
                _logger.Error("Failed to retrieve setups information.");
                return false;
            }

            DownloadedSetupPath = await GetSetup(setupToDownload, cancellationToken, progress);

            return !string.IsNullOrEmpty(DownloadedSetupPath);
        }
        public async Task<bool> StartDownloadedSetup()
        {
            if (string.IsNullOrEmpty(DownloadedSetupPath))
                return false;

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    FileName = DownloadedSetupPath,
                    Arguments = _environment.CommandLine
                };
                await Task.Run(() => _processStarter.StartWithSameElevation(startInfo));

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while starting the downloaded setup.");
                return false;
            }
        }

        private async Task<DownloadableSetupInfo> GetSetupInfo(string url, string sectionName)
        {
            var downloadableSetupInfo = new DownloadableSetupInfo();

            try
            {
                var latestSetupInfo = await _downloader.GetStringAsync(url);

                await using var stream = CreateStreamFromString(latestSetupInfo);
                _logger.Debug("Parsing setup info");
                var data = Data.CreateDataStorage();
                var iniStorage = new IniStorage("");
                iniStorage.ReadData(stream, data);

                downloadableSetupInfo.DownloadUrl = data.GetValue(sectionName + "\\DownloadUrl");
                downloadableSetupInfo.FileHash = data.GetValue(sectionName + "\\FileHash");
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Error while downloading update-info file.");
            }

            return downloadableSetupInfo;
        }

        private async Task<string> GetSetup(DownloadableSetupInfo setupToDownload, CancellationToken cancellationToken, Action<int> progress)
        {
            var tempSetupFilename = Path.ChangeExtension(_path.GetTempFileName(), ".exe");
            try
            {
                await _downloader.DownloadFileAsync(
                    setupToDownload.DownloadUrl,
                    tempSetupFilename,
                    cancellationToken,
                    progress);

                if (cancellationToken.IsCancellationRequested)
                {
                    TryDeleteTempFile(tempSetupFilename);
                    return null;
                }

                _logger.Info("Download completed");

                var downloadedFileHash = _hashUtil.CalculateFileMd5(tempSetupFilename);
                if (string.Compare(downloadedFileHash, setupToDownload.FileHash, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    _logger.Warn("The MD5 hashes do not match!");
                    TryDeleteTempFile(tempSetupFilename);
                    return null;
                }
            }
            catch (WebException ex)
            {
                _logger.Warn(ex, "Error while downloading setup file");
                TryDeleteTempFile(tempSetupFilename);
            }

            return tempSetupFilename;
        }

        private bool StartSetup(string pdfCreatorSetupFilename, string args)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    FileName = pdfCreatorSetupFilename,
                    Arguments = args
                };
                _processStarter.StartWithSameElevation(startInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while trying to open the setup file at: {pdfCreatorSetupFilename}");
                TryDeleteTempFile(pdfCreatorSetupFilename);
                return false;
            }

            return true;
        }
        private static Stream CreateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private void TryDeleteTempFile(string path)
        {
            try
            {
                _logger.Warn("Deleting temporary file {0}", path);
                _file.Delete(path);
            }
            catch
            {
                _logger.Error("Could not delete temporary file {0}", path);
            }
        }
    }
}
