using Microsoft.Win32;
using NLog;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.Extensions;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class UpdateHelper : IUpdateHelper
    {
        private readonly IGpoSettings _gpoSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IOnlineVersionHelper _onlineVersionHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICurrentSettings<ApplicationSettings> _settingsProvider;

        private readonly string _skipVersionRegistryPath;
        private readonly IVersionHelper _versionHelper;
        private bool _showUpdateDuringSession = false;
        private bool _isTimeForNextUpdate = false;

        public UpdateHelper(ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            IVersionHelper versionHelper, IInstallationPathProvider installationPathProvider, IGpoSettings gpoSettings,
            IEventAggregator eventAggregator, IOnlineVersionHelper onlineVersionHelper)
        {
            _settingsProvider = applicationSettingsProvider;
            _versionHelper = versionHelper;
            _gpoSettings = gpoSettings;
            _eventAggregator = eventAggregator;
            _onlineVersionHelper = onlineVersionHelper;
            _skipVersionRegistryPath = installationPathProvider.RegistryHive + "\\" + installationPathProvider.ApplicationRegistryPath;
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
            set => _settingsProvider.Settings.NextUpdate = value;
        }

        private UpdateInterval UpdateInterval
        {
            get
            {
                if (string.IsNullOrEmpty(_gpoSettings?.UpdateInterval))
                    return _settingsProvider.Settings.UpdateInterval;
                return UpdateIntervalHelper.ParseUpdateInterval(_gpoSettings.UpdateInterval);
            }
        }

        /// <summary>
        ///     Flag to report, if UpdateProcedure(Thread) is already running
        /// </summary>
        public bool UpdateProcedureIsRunning { get; private set; }

        public bool UpdatesEnabled => true;

        public async Task UpdateCheckAsync(bool checkNecessity)
        {
            var eventArgs = new UpdateEventArgs();
            if (UpdateProcedureIsRunning)
            {
                return;
            }

            UpdateProcedureIsRunning = true;

            try
            {
                await Update(eventArgs, checkNecessity);
            }
            catch (Exception e)
            {
                _logger.Error("Exception in UpdateProcedure:\r\n" + e.Message);
            }

            UpdateProcedureIsRunning = false;
        }

        public bool IsTimeForNextUpdate()
        {
            if (_showUpdateDuringSession || _isTimeForNextUpdate)
                return true;

            if (UpdateInterval == UpdateInterval.Never)
            {
                _logger.Debug("Automatic UpdateCheck is disabled");
                return false;
            }

            if (DateTime.Now < NextUpdate)
            {
                _logger.Debug("Update period did not pass. Next Update is set to: " + NextUpdate);
                return false;
            }

            _isTimeForNextUpdate = true;
            return true;
        }

        public bool UpdateShouldBeShown()
        {
            if (!IsTimeForNextUpdate())
                return false;

            SetNewUpdateTime();

            if (!IsUpdateAvailable() || !VersionShouldNotBeSkipped(true, _onlineVersionHelper.GetOnlineVersion().Version))
                return false;

            _logger.Debug("Update period has passed (set to: " + NextUpdate + ")");

            _showUpdateDuringSession = true;

            return true;
        }

        public void ShowLater()
        {
            SetNewUpdateTime();

            _isTimeForNextUpdate = false;
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
        }

        private async Task Update(UpdateEventArgs eventArgs, bool checkNecessity)
        {
            if (!IsTimeForNextUpdate())
            {
                return;
            }

            IApplicationVersion onlineVersion = await _onlineVersionHelper.LoadOnlineVersionAsync();

            if (checkNecessity)
            {
                if (!UpdateShouldBeShown())
                    return;
            }

            _logger.Debug("Start UpdateCheck");

            var thisVersion = _versionHelper.ApplicationVersion;

            if (onlineVersion == null)
            {
                _logger.Error("OnlineVersion not available");
                return;
            }

            if (thisVersion.CompareTo(onlineVersion.Version) < 0 && VersionShouldNotBeSkipped(checkNecessity, onlineVersion.Version))
            {
                _logger.Info("New Version available");
                _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(true);
                _eventAggregator.GetEvent<ShowUpdateInteractionEvent>().Publish();
            }

            if (eventArgs.SkipVersion)
            {
                SetSkipVersionInRegistry(onlineVersion.Version);
            }

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
            var timeSpan = UpdateInterval.ToTimeSpan();
            NextUpdate = DateTime.Now.Add(timeSpan);
            _showUpdateDuringSession = false;
        }

        public void SkipVersion()
        {
            var onlineVersion = _onlineVersionHelper.GetOnlineVersion();

            if (onlineVersion != null)
                SetSkipVersionInRegistry(onlineVersion.Version);

            SetNewUpdateTime();
            _isTimeForNextUpdate = false;
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
        }

        public async Task<bool> IsUpdateAvailableAsync(bool checkNecessity)
        {
            if (checkNecessity && !UpdateShouldBeShown())
                return false;

            NextUpdate = DateTime.Now;
            SetSkipVersionInRegistry(_versionHelper.ApplicationVersion);
            await _onlineVersionHelper.LoadOnlineVersionAsync(true);
            _eventAggregator.GetEvent<ShowUpdateInteractionEvent>().Publish();
            return IsUpdateAvailable();
        }

        public bool IsUpdateAvailable()
        {
            if (_onlineVersionHelper.GetOnlineVersion() == null)
                return false;

            var thisVersion = _versionHelper.ApplicationVersion;

            var updateAvailable = thisVersion.CompareTo(_onlineVersionHelper.GetOnlineVersion().Version) < 0;
            return updateAvailable;
        }
    }
}
