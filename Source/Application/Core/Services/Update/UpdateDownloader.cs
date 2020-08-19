using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Services.Update
{
    public class UpdateDownloader : IUpdateDownloader
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectory _directory;
        private readonly IFile _systemFile;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IHashUtil _hashUtil;
        private readonly ICancellationTokenSourceFactory _cancellationSourceFactory;

        private HttpClient _httpClient;
        public DownloadSpeed DownloadSpeed { get; set; }

        public event EventHandler<UpdateProgressChangedEventArgs> OnDownloadFinished;

        public event EventHandler<UpdateProgressChangedEventArgs> OnProgressChanged;

        private Task _downloadTask;
        private CancellationTokenSource _cancellationSource;

        public UpdateDownloader(IDirectory directory, IFile systemFile, ITempFolderProvider tempFolderProvider, IHashUtil hashUtil, ICancellationTokenSourceFactory cancellationSourceFactory)
        {
            _directory = directory;
            _systemFile = systemFile;
            _tempFolderProvider = tempFolderProvider;
            _hashUtil = hashUtil;
            _cancellationSourceFactory = cancellationSourceFactory;
        }

        public string GetDownloadPath(string downloadUrl)
        {
            var downloadLocation = _tempFolderProvider.TempFolder;
            _directory.CreateDirectory(downloadLocation);
            var uri = new Uri(downloadUrl);
            var filename = PathSafe.GetFileName(uri.LocalPath);
            return PathSafe.Combine(downloadLocation, filename);
        }

        public async Task StartDownloadAsync(IApplicationVersion version)
        {
            if (_downloadTask == null)
            {
                _cancellationSource = _cancellationSourceFactory.CreateSource();
                _downloadTask = Task.Run(() =>
                {
                    _httpClient = new HttpClient();

                    DownloadSpeed = new DownloadSpeed();

                    OnProgressChanged += DownloadSpeed.DownloadProgressChanged;
                    OnDownloadFinished += DownloadSpeed.webClient_DownloadFileCompleted;

                    var downloadFileWithRange = DownloadFileWithRange(version);
                    OnDownloadFinished?.Invoke(this, new UpdateProgressChangedEventArgs(downloadFileWithRange, 0, 0, 0));
                    _downloadTask = null;
                }, _cancellationSource.Token);
            }

            await _downloadTask;
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
                long requestContentLength = 0;
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(uri);

                    if (totalBytesRead > 0)
                        request.AddRange(totalBytesRead);

                    var response = request.GetResponse();

                    if (response.ContentLength > maxContentLength)
                        maxContentLength = response.ContentLength + totalBytesRead;

                    if (maxContentLength <= totalBytesRead)
                        return true;

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
                    _logger.Error(ex, $"Error while downloading; Got bytes: {requestContentLength} with Error:{ex.Message}");
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
            if (_cancellationSource.Token.CanBeCanceled)
            {
                _httpClient?.CancelPendingRequests();
                _cancellationSource.Cancel();
            }
        }
    }

    public interface IUpdateDownloader
    {
        Task StartDownloadAsync(IApplicationVersion version);

        event EventHandler<UpdateProgressChangedEventArgs> OnDownloadFinished;

        void AbortDownload();

        event EventHandler<UpdateProgressChangedEventArgs> OnProgressChanged;

        bool IsDownloaded(string filePath);

        string GetDownloadPath(string downloadUrl);

        DownloadSpeed DownloadSpeed { get; set; }
    }
}
