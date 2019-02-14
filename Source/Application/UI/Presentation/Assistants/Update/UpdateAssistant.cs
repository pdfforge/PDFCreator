using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Update;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class UpdateAssistant : IUpdateAssistant
    {
        private readonly UpdateInformationProvider _updateInformationProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUpdateChangeParser _changeParser;
        private readonly IUpdateHelper _updateHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICurrentSettings<ApplicationSettings> _settingsProvider;

        private readonly string _skipVersionRegistryPath;
        private readonly IThreadManager _threadManager;
        private UpdateManagerTranslation _translation;
        private readonly IVersionHelper _versionHelper;

        public Release CurrentReleaseVersion
        {
            get => _updateHelper.CurrentReleaseVersion;
            set => _updateHelper.CurrentReleaseVersion = value;
        }

        public UpdateAssistant(ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            ITranslationUpdater translationUpdater, IThreadManager threadManager, IVersionHelper versionHelper,
            IInstallationPathProvider installationPathProvider, IUpdateLauncher updateLauncher, UpdateInformationProvider updateInformationProvider,
            IGpoSettings gpoSettings, IEventAggregator eventAggregator, IUpdateChangeParser changeParser, IUpdateHelper updateHelper)
        {
            _settingsProvider = applicationSettingsProvider;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            _threadManager = threadManager;
            _versionHelper = versionHelper;
            _updateInformationProvider = updateInformationProvider;
            _gpoSettings = gpoSettings;
            _eventAggregator = eventAggregator;
            _changeParser = changeParser;
            _updateHelper = updateHelper;
            _skipVersionRegistryPath = installationPathProvider.RegistryHive + "\\" + installationPathProvider.ApplicationRegistryPath;
        }

        public event EventHandler TryShowUpdateInteraction;

        /// <summary>
        ///     onlineVersion of the last UpdateProcedure
        /// </summary>
        public IApplicationVersion OnlineVersion
        {
            get => _updateHelper.OnlineVersion;
            private set => _updateHelper.OnlineVersion = value;
        }

        /// <summary>
        ///     Current time for the next automated updateCheck
        /// </summary>
        private DateTime NextUpdate
        {
            get
            {
                if (_settingsProvider.Settings != null)
                {
                    return _settingsProvider.Settings.NextUpdate;
                }

                return DateTime.Now;
            }
            set { _settingsProvider.Settings.NextUpdate = value; }
        }

        public bool ShowUpdate => NextUpdate <= DateTime.Now && IsUpdateAvailable();

        private UpdateInterval UpdateInterval
        {
            get
            {
                if (_gpoSettings?.UpdateInterval == null)
                    return _settingsProvider.Settings.UpdateInterval;
                return UpdateIntervalHelper.ParseUpdateInterval(_gpoSettings.UpdateInterval);
            }
        }

        /// <summary>
        ///     Flag to report, if UpdateProcedure(Thread) is already running
        /// </summary>
        public bool UpdateProcedureIsRunning { get; private set; }

        public bool UpdatesEnabled => true;

        /// <summary>
        ///     Initialize with NextUpdate and UpdateInterval from Settings
        /// </summary>
        /// <summary>
        ///     Launch UpdateProcedure in separate Thread.
        ///     UpdateManager must be initialized!
        ///     Downloads update-info.txt and compares the recent (online) version to the current version
        ///     and launches assigned events.
        ///     Specific Events must be set in advance.
        ///     Can only be launched once at the time, reported in UpdateProcedureIsRunning flag.
        ///     Resets the UpdateManager afterwards.
        /// </summary>
        /// <param name="checkNecessity"></param>
        public void UpdateProcedure(bool checkNecessity)
        {
            var thread = new SynchronizedThread(() => UpdateThread(checkNecessity));
            thread.Name = "UpdateThread";
            thread.SetApartmentState(ApartmentState.STA);
            _threadManager.StartSynchronizedThread(thread);
        }

        private void UpdateThread(bool checkNecessity)
        {
            _logger.Debug("Launched UpdateThread");
            var eventArgs = new UpdateEventArgs();
            if (UpdateProcedureIsRunning)
            {
                _logger.Debug("UpdateThread is already running");
                return;
            }
            UpdateProcedureIsRunning = true;

            try
            {
                Update(eventArgs, checkNecessity);
            }
            catch (Exception e)
            {
                _logger.Error("Exception in UpdateProcedure:\r\n" + e.Message);
            }

            UpdateProcedureIsRunning = false;
        }

        private bool UpdateShouldBeShown()
        {
            if (UpdateInterval == UpdateInterval.Never)
            {
                _logger.Debug("Automatic UpdateCheck is disabled");
                return false;
            }

            if (DateTime.Compare(DateTime.Now, NextUpdate) < 0)
            {
                _logger.Debug("Update period did not pass. Next Update is set to: " + NextUpdate);
                return false;
            }
            _logger.Debug("Update period has passed (set to: " + NextUpdate + ")");

            return true;
        }

        private void Update(UpdateEventArgs eventArgs, bool checkNecessity)
        {
            if (checkNecessity)
            {
                if (!UpdateShouldBeShown())
                    return;
            }

            _logger.Debug("Start UpdateCheck");

            var thisVersion = _versionHelper.ApplicationVersion;
            var onlineVersion = GetOnlineVersion();
            if (onlineVersion == null)
            {
                _logger.Error("OnlineVersion not available");
                return;
            }

            if (thisVersion.CompareTo(onlineVersion.Version) < 0 && VersionShouldNotBeSkipped(checkNecessity, onlineVersion.Version))
            {
                _logger.Info("New Version available");
                _updateHelper.OnlineVersion = onlineVersion;
                _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(true);
                TryShowUpdateInteraction?.Invoke(this, EventArgs.Empty);
            }

            if (eventArgs.SkipVersion)
            {
                SetSkipVersionInRegistry(OnlineVersion.Version);
            }

            _settingsProvider.Settings.NextUpdate = NextUpdate;

            _logger.Info("NextUpdate dated to " + NextUpdate);
        }

        private bool VersionShouldNotBeSkipped(bool checkNecessity, Version onlineVersion)
        {
            if (!checkNecessity)
                return true;

            var skipVersion = ReadSkipVersionFromRegistry();
            return skipVersion.CompareTo(onlineVersion) < 0;
        }

        private Version ReadSkipVersionFromRegistry()
        {
            var versionString = Registry.GetValue(_skipVersionRegistryPath, "SkipVersion", "")?.ToString();
            if (string.IsNullOrWhiteSpace(versionString))
                return new Version(0, 0);

            var success = Version.TryParse(versionString, out var version);

            return success ? version : new Version(0, 0);
        }

        private void SetSkipVersionInRegistry(Version onlineVersion)
        {
            Registry.SetValue(_skipVersionRegistryPath, "SkipVersion", onlineVersion.ToString());
        }

        public void SetNewUpdateTime()
        {
            switch (UpdateInterval)
            {
                default:
                    NextUpdate = DateTime.Now.AddDays(7);
                    break;

                case UpdateInterval.Daily:
                    NextUpdate = DateTime.Now.AddDays(1);
                    break;

                case UpdateInterval.Weekly:
                    NextUpdate = DateTime.Now.AddDays(7);
                    break;

                case UpdateInterval.Monthly:
                    NextUpdate = DateTime.Now.AddMonths(1);
                    break;

                case UpdateInterval.Never:
                    NextUpdate = DateTime.Now.AddYears(10);
                    break;
            }

            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
        }

        private ApplicationVersion GetOnlineVersion()
        {
            _logger.Debug("Get online Version");

            var url = _updateInformationProvider.UpdateInfoUrl;
            var sectionName = _updateInformationProvider.SectionName;
            try
            {
                using (var webClient = new WebClient())
                {
                    var contents = webClient.DownloadString(url);
                    using (var stream = CreateStreamFromString(contents))
                    {
                        _logger.Debug("Loading update-info.txt");
                        var data = Data.CreateDataStorage();
                        var iniStorage = new IniStorage("");
                        iniStorage.ReadData(stream, data);

                        var onlineVersion = new Version(data.GetValue(sectionName + "\\Version"));
                        var downloadUrl = data.GetValue(sectionName + "\\DownloadUrl");
                        var fileHash = data.GetValue(sectionName + "\\FileHash");
                        _logger.Info("Online Version: " + onlineVersion);

                        var versionsInfo = new List<Release>();
                        var applicationVersion = _versionHelper.ApplicationVersion;

                        if (applicationVersion.CompareTo(onlineVersion) < 0)
                        {
                            var availableInfos = _changeParser.Parse(webClient.DownloadString(Urls.PdfCreatorUpdateChangelogUrl));
                            versionsInfo = availableInfos.FindAll(release => release.Version > applicationVersion);

                            CurrentReleaseVersion = availableInfos.FirstOrDefault(x => x.Version.IsEqualToCurrentVersion(applicationVersion));
                        }

                        return new ApplicationVersion(onlineVersion, downloadUrl, fileHash, versionsInfo);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Warn(e.Message);

                return new ApplicationVersion(new Version(0, 0, 0), "", "", new List<Release>()); ;
            }
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

        public void SkipVersion()
        {
            if (OnlineVersion != null)
                SetSkipVersionInRegistry(OnlineVersion.Version);
            SetNewUpdateTime();
            _settingsProvider.Settings.NextUpdate = NextUpdate;
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
        }

        public bool IsOnlineUpdateAvailable(bool checkNecessity)
        {
            if (checkNecessity && !UpdateShouldBeShown())
                return false;

            NextUpdate = DateTime.Now;
            _updateHelper.OnlineVersion = GetOnlineVersion();
            TryShowUpdateInteraction?.Invoke(this, EventArgs.Empty);
            return IsUpdateAvailable();
        }

        public bool IsUpdateAvailable()
        {
            if (OnlineVersion == null)
                return false;

            var thisVersion = _versionHelper.ApplicationVersion;

            var updateAvailable = thisVersion.CompareTo(OnlineVersion.Version) < 0;

            return updateAvailable;
        }

        public void DownloadUpdate()
        {
            if (!UpdateShouldBeShown())
                return;

            var onlineVersion = GetOnlineVersion();
            if (onlineVersion == null)
            {
                _logger.Error("OnlineVersion not available");
                return;
            }

            var thisVersion = _versionHelper.ApplicationVersion;
            if (thisVersion.CompareTo(onlineVersion.Version) < 0)
            {
                _updateHelper.StartDownload(onlineVersion);
                _updateHelper.OnlineVersion = onlineVersion;
            }
        }
    }
}
