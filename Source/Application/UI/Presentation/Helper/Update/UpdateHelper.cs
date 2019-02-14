using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Update
{
    public class UpdateHelper : IUpdateHelper
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectory _directory;
        private readonly IFile _systemFile;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IHashUtil _hashUtil;

        public string DownloadLocation { get; private set; }
        private HttpClient _httpClient;
        public DownloadSpeed DownloadSpeed { get; set; }

        public event EventHandler<UpdateProgressChangedEventArgs> OnDownloadFinished;

        public event EventHandler<UpdateProgressChangedEventArgs> OnProgressChanged;

        private bool isDownloading;

        /// <summary>
        ///     onlineVersion of the last UpdateProcedure
        /// </summary>
        public IApplicationVersion OnlineVersion { get; set; }

        public Release CurrentReleaseVersion { get; set; }

        public UpdateHelper(IDirectory directory, IFile systemFile, ITempFolderProvider tempFolderProvider, IHashUtil hashUtil)
        {
            _directory = directory;
            _systemFile = systemFile;
            _tempFolderProvider = tempFolderProvider;
            _hashUtil = hashUtil;
            OnlineVersion = new ApplicationVersion(new Version(0, 0, 0, 0), "", "", new List<Release>());
        }

        public string GetDownloadPath(string downloadUrl)
        {
            var downloadLocation = _tempFolderProvider.TempFolder;
            _directory.CreateDirectory(downloadLocation);
            var uri = new Uri(downloadUrl);
            var filename = PathSafe.GetFileName(uri.LocalPath);
            return PathSafe.Combine(downloadLocation, filename);
        }

        public void StartDownload(IApplicationVersion version)
        {
            if (isDownloading)
                return;

            _httpClient = new HttpClient();

            Task.Run(() =>
            {
                isDownloading = true;
                DownloadSpeed = new DownloadSpeed();

                OnProgressChanged += DownloadSpeed.DownloadProgressChanged;
                OnDownloadFinished += DownloadSpeed.webClient_DownloadFileCompleted;

                var downloadFileWithRange = DownloadFileWithRange(version);
                OnDownloadFinished?.Invoke(this, new UpdateProgressChangedEventArgs(downloadFileWithRange, 0, 0, 0));
                isDownloading = false;
            });
        }

        public bool IsDownloaded(string filePath)
        {
            // file is already downloaded
            if (_systemFile.Exists(filePath))
                return true;
            return false;
        }

        private bool DownloadFileWithRange(IApplicationVersion version)
        {
            var uri = new Uri(version.DownloadUrl);
            var filePath = GetDownloadPath(version.DownloadUrl);

            long totalBytesRead = 0;
            string tempFile = filePath + ".temp";

            // file is already downloaded and renamed
            if (IsDownloaded(filePath))
                return true;

            var fileInfo = new FileInfo(tempFile);
            if (fileInfo.Exists)
            {
                if (_hashUtil.VerifyFileMd5(tempFile, version.FileHash))
                {
                    // File was downloaded already but not renamed
                    File.Move(tempFile, filePath);
                    return true;
                }
                totalBytesRead = fileInfo.Length;
            }

            long maxContentLength = 0;
            while (maxContentLength == 0 || totalBytesRead < maxContentLength)
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);

                if (totalBytesRead > 0)
                    request.AddRange(totalBytesRead);

                var response = request.GetResponse();

                if (response.ContentLength > maxContentLength)
                    maxContentLength = response.ContentLength + totalBytesRead;

                if (maxContentLength <= totalBytesRead)
                    return true;

                long requestContentLength = 0;
                try
                {
                    using (var responseStream = response.GetResponseStream())
                    using (var localFileStream = _systemFile.Open(tempFile, FileMode.Append, FileAccess.Write))
                    {
                        var buffer = new byte[4096 * 4];
                        int bytesRead;

                        while (responseStream != null && (bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead += bytesRead;
                            requestContentLength += bytesRead;
                            var progressInPercent = (int)(totalBytesRead * 100 / maxContentLength);
                            OnProgressChanged?.Invoke(this, new UpdateProgressChangedEventArgs(false, progressInPercent, totalBytesRead, maxContentLength));
                            localFileStream.Write(buffer, 0, bytesRead);
                        }
                        _logger.Debug("Got bytes: {0}", requestContentLength);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error while downloading; Got bytes: {requestContentLength} with Error:{ex.Message}");
                }
            }

            if (maxContentLength == totalBytesRead)
            {
                _systemFile.Move(tempFile, filePath);
                return true;
            }

            return false;
        }

        public void AbortDownload()
        {
            _httpClient?.CancelPendingRequests();
        }
    }

    public interface IUpdateHelper
    {
        void StartDownload(IApplicationVersion version);

        event EventHandler<UpdateProgressChangedEventArgs> OnDownloadFinished;

        string DownloadLocation { get; }

        void AbortDownload();

        event EventHandler<UpdateProgressChangedEventArgs> OnProgressChanged;

        bool IsDownloaded(string filePath);

        string GetDownloadPath(string downloadUrl);

        IApplicationVersion OnlineVersion { get; set; }
        Release CurrentReleaseVersion { get; set; }
        DownloadSpeed DownloadSpeed { get; set; }
    }
}
