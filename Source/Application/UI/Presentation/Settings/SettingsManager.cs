using System;
using System.Management;
using System.Security.Principal;
using NLog;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.Settings
{
    public class PDFCreatorSettingsManager : SettingsManager
    {
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly IThreadManager _threadManager;
        private readonly IGpoSettings _gpoSettings;
        private readonly IPrinterMappingsHelper _printerMappingsHelper;
        private bool _registrySettingsHaveChanged;
        private ManagementEventWatcher _registryWatcher;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PDFCreatorSettingsManager(ISettingsProvider settingsProvider, ISettingsLoader loader, 
            IInstallationPathProvider installationPathProvider, IThreadManager threadManager, 
            IGpoSettings gpoSettings, IPrinterMappingsHelper printerMappingsHelper) 
            : base(settingsProvider, loader, installationPathProvider)
        {
            _installationPathProvider = installationPathProvider;
            _threadManager = threadManager;
            _gpoSettings = gpoSettings;
            _printerMappingsHelper = printerMappingsHelper;

            threadManager.StandbyStarted += OnStandbyStarted;

            threadManager.StandbyEnded += OnStandbyEnded;
        }

        private void OnStandbyStarted(object sender, EventArgs args)
        {
            if (_registryWatcher == null) _registryWatcher = BuildRegistryWatcher();

            SaveCurrentSettings();

            _registrySettingsHaveChanged = false;

            try
            {
                _logger.Trace("Starting to watch the registry for changes during standby");
                _registryWatcher.Start();
            }
            catch (ManagementException ex)
            {
                _logger.Warn(ex, "Could not watch the registry for changes");
                _registryWatcher = null;
            }
        }

        private void OnStandbyEnded(object sender, EventArgs args)
        {
            _logger.Trace("Stopping to watch the registry for changes during standby");

            _registryWatcher?.Stop();

            if (_registrySettingsHaveChanged || _registryWatcher == null) LoadAllSettings();

            _printerMappingsHelper.CheckPrinterMappings(SettingsProvider.Settings);
        }

        private ManagementEventWatcher BuildRegistryWatcher()
        {
            var escapedPath = _installationPathProvider.ApplicationRegistryPath.Replace(@"\", @"\\");

            var currentUserId = WindowsIdentity.GetCurrent().User?.Value ?? "";
            currentUserId = currentUserId.Replace(@"\", @"\\");

            // WMI does not support watching HKEY_CURRENT_USER, so we use the current user's key in HKEY_USERS
            var query = "SELECT * FROM RegistryTreeChangeEvent " +
                        @"WHERE Hive='HKEY_USERS' " +
                        $@"AND RootPath='{currentUserId}\\{escapedPath}'";

            var watcher = new ManagementEventWatcher(query);

            var handler = new EventArrivedEventHandler((sender, e) => _registrySettingsHaveChanged = true);
            watcher.EventArrived += handler;

            return watcher;
        }

        protected override void ProcessAfterLoading(PdfCreatorSettings settings)
        {
            LoggingHelper.ChangeLogLevel(settings.ApplicationSettings.LoggingLevel);
            SetHotStandbyDuration(settings.CreatorAppSettings);
        }

        protected override void ProcessBeforeSaving(PdfCreatorSettings settings) { }

        protected override void ProcessAfterSaving(PdfCreatorSettings settings)
        {
            LoggingHelper.ChangeLogLevel(settings.ApplicationSettings.LoggingLevel);
            SetHotStandbyDuration(settings.CreatorAppSettings);
        }

        private void SetHotStandbyDuration(CreatorAppSettings settings)
        {
            var standbyMinutes = settings.HotStandbyMinutes;

            if (_gpoSettings.HotStandbyMinutes.HasValue)
                standbyMinutes = _gpoSettings.HotStandbyMinutes.Value;

            if (standbyMinutes < 0)
                standbyMinutes = 0;

            _threadManager.HotStandbyDuration = TimeSpan.FromMinutes(standbyMinutes);
        }
    }
}