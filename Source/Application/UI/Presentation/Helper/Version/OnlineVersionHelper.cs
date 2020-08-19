using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Extensions;
using pdfforge.PDFCreator.Core.Services.Cache;
using pdfforge.PDFCreator.Core.Services.Download;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Version
{
    public class OnlineVersionHelper : IOnlineVersionHelper
    {
        private readonly UpdateInformationProvider _updateInformationProvider;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IUpdateChangeParser _changeParser;
        private readonly IFileCacheFactory _fileCacheFactory;
        private readonly IDownloader _downloader;
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IApplicationVersion _onlineVersion;
        private readonly FileCache _fileCache;

        public Release CurrentReleaseVersion { get; private set; }

        public OnlineVersionHelper(UpdateInformationProvider updateInformationProvider, ITempFolderProvider tempFolderProvider, IVersionHelper versionHelper, IUpdateChangeParser changeParser, IFileCacheFactory fileCacheFactory, IDownloader downloader, ICurrentSettings<ApplicationSettings> applicationSettingsProvider)
        {
            _updateInformationProvider = updateInformationProvider;
            _tempFolderProvider = tempFolderProvider;
            _versionHelper = versionHelper;
            _changeParser = changeParser;
            _fileCacheFactory = fileCacheFactory;
            _downloader = downloader;
            _applicationSettingsProvider = applicationSettingsProvider;

            if (_applicationSettingsProvider != null)
            {
                _fileCache = GetFileCache();
                _applicationSettingsProvider.SettingsChanged += ApplicationSettingsProviderOnSettingsChanged;
            }
            _onlineVersion = new ApplicationVersion(new System.Version(0, 0, 0, 0), "", "", new List<Release>());
        }

        private void ApplicationSettingsProviderOnSettingsChanged(object sender, EventArgs e)
        {
            var settingsUpdateInterval = _applicationSettingsProvider.Settings.UpdateInterval;
            _fileCache.SetMaxDuration(settingsUpdateInterval.ToTimeSpan());
        }

        private FileCache GetFileCache()
        {
            var updateInterval = _applicationSettingsProvider.Settings.UpdateInterval;
            var cacheDirectory = Path.Combine(_tempFolderProvider.TempFolder, "Update");
            return _fileCacheFactory.GetFileCache(cacheDirectory, updateInterval.ToTimeSpan());
        }

        private Task<IApplicationVersion> _loadingOnlineVersionTask;
        private readonly SemaphoreSlim _loadingOnlineVersionSemaphore = new SemaphoreSlim(1);

        public async Task<IApplicationVersion> LoadOnlineVersionAsync(bool forceDownload = false, bool onlyCache = false)
        {
            await _loadingOnlineVersionSemaphore.WaitAsync();
            try
            {
                if (_loadingOnlineVersionTask == null || forceDownload)
                {
                    _loadingOnlineVersionTask = LoadOnlineVersionAsyncInternal(forceDownload, onlyCache);
                }
            }
            finally
            {
                _loadingOnlineVersionSemaphore.Release();
            }

            return await _loadingOnlineVersionTask;
        }

        private async Task<IApplicationVersion> LoadOnlineVersionAsyncInternal(bool forceDownload = false, bool onlyCache = false)
        {
            _logger.Debug("Get online Version");

            var url = _updateInformationProvider.UpdateInfoUrl;
            var sectionName = _updateInformationProvider.SectionName;
            try
            {
                var contents = await RetrieveFileFromCacheOrUrl(url, "update-info.txt", forceDownload);

                using (var stream = CreateStreamFromString(contents))
                {
                    _logger.Debug("Loading update-info.txt");
                    var data = Data.CreateDataStorage();
                    var iniStorage = new IniStorage("");
                    iniStorage.ReadData(stream, data);

                    var onlineVersion = new System.Version(data.GetValue(sectionName + "\\Version"));
                    var downloadUrl = data.GetValue(sectionName + "\\DownloadUrl");
                    var fileHash = data.GetValue(sectionName + "\\FileHash");
                    _logger.Info("Online Version: " + onlineVersion);

                    var versionsInfo = new List<Release>();
                    var applicationVersion = _versionHelper.ApplicationVersion;

                    if (applicationVersion.CompareTo(onlineVersion) < 0)
                    {
                        var downloadString = await RetrieveFileFromCacheOrUrl(_updateInformationProvider.ChangeLogUrl, "Releases.json", forceDownload);
                        var availableInfos = _changeParser.Parse(downloadString);
                        versionsInfo = availableInfos.FindAll(release => release.Version > applicationVersion);

                        CurrentReleaseVersion = availableInfos.FirstOrDefault(x => x.Version.IsEqualToCurrentVersion(applicationVersion));
                    }

                    _onlineVersion = new ApplicationVersion(onlineVersion, downloadUrl, fileHash, versionsInfo);
                }
            }
            catch (Exception e)
            {
                _logger.Warn(e.Message);

                _onlineVersion = new ApplicationVersion(new System.Version(0, 0, 0), "", "", new List<Release>());
            }

            return _onlineVersion;
        }

        public IApplicationVersion GetOnlineVersion()
        {
            return _onlineVersion;
        }

        private Stream CreateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async Task<string> RetrieveFileFromCacheOrUrl(string url, string filename, bool forceDownload)
        {
            string returnVal;
            try
            {
                if (_fileCache.FileAvailable(filename) && !forceDownload)
                {
                    // File was cached and can be used
                    returnVal = _fileCache.ReadFile(filename);
                }
                else
                {
                    // File was not cached yet or is out-dated
                    returnVal = await _downloader.DownloadStringTaskAsync(url);
                    await _fileCache.SaveFileAsync(filename, CreateStreamFromString(returnVal));
                }
            }
            catch (Exception e)
            {
                var errorMessage = $"Error while downloading file:{filename}";
                _logger.Error(errorMessage + "\n" + e.Message);

                // Error while getting proper file, try for outdated file in cache
                returnVal = GetOutDatedCachedFile(filename);

                if (string.IsNullOrEmpty(returnVal))
                    throw new Exception(errorMessage, e);
            }

            return returnVal;
        }

        private string GetOutDatedCachedFile(string filename)
        {
            try
            {
                if (_fileCache.FileAvailable(filename, true))
                {
                    // outdated file was cached and can be used
                    var tryGetOutDatedCachedFile = _fileCache.ReadFile(filename);
                    return tryGetOutDatedCachedFile;
                }
            }
            catch (Exception e)
            {
                _logger.Error($@"Error while loading outdated cached file {filename} \n {e.Message}");
            }

            return "";
        }
    }

    public class DisabledOnlineVersionHelper : IOnlineVersionHelper
    {
        private readonly IVersionHelper _versionHelper;

        public DisabledOnlineVersionHelper(IVersionHelper versionHelper)
        {
            _versionHelper = versionHelper;
        }

        public Release CurrentReleaseVersion { get; } = new Release();

        public IApplicationVersion GetOnlineVersion()
        {
            return new ApplicationVersion(_versionHelper.ApplicationVersion, "", "", new List<Release>()) as IApplicationVersion;
        }

        public Task<IApplicationVersion> LoadOnlineVersionAsync(bool force = false, bool onlyCache = false)
        {
            return Task.FromResult(new ApplicationVersion(_versionHelper.ApplicationVersion, "", "", new List<Release>()) as IApplicationVersion);
        }
    }

    public interface IOnlineVersionHelper
    {
        Release CurrentReleaseVersion { get; }

        Task<IApplicationVersion> LoadOnlineVersionAsync(bool force = false, bool onlyCache = false);

        IApplicationVersion GetOnlineVersion();
    }
}
