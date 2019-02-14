using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using System;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class PdfCreatorUsageStatisticsManager : IPdfCreatorUsageStatisticsManager
    {
        private readonly IUsageStatisticsSender _sender;
        private readonly IOsHelper _osHelper;
        private readonly IMachineIdGenerator _machineIdGenerator;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly IThreadManager _threadManager;

        public PdfCreatorUsageStatisticsManager(IUsageStatisticsSender sender, IOsHelper osHelper, IMachineIdGenerator machineIdGenerator,
            ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper,
            ISettingsProvider settingsProvider, IGpoSettings gpoSettings, IThreadManager threadManager)
        {
            _sender = sender;
            _osHelper = osHelper;
            _machineIdGenerator = machineIdGenerator;
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
            _settingsProvider = settingsProvider;
            _gpoSettings = gpoSettings;
            _threadManager = threadManager;
        }

        private bool IsEnabled => !_gpoSettings.DisableUsageStatistics && _settingsProvider.Settings.ApplicationSettings.UsageStatistics.Enable;

        public void SendUsageStatistics(TimeSpan duration, Job job, string status)
        {
            if (IsEnabled)
            {
                var usageMetric = CreateJobUsageStatisticsMetric(job, duration, status);

                var senderThread = new SynchronizedThread(() => _sender.Send(usageMetric));

                _threadManager.StartSynchronizedThread(senderThread);
            }
        }

        public bool IsComMode { get; set; }

        private string GetApplicationName()
        {
            return _applicationNameProvider.ApplicationNameWithEdition.ToLower().Replace(" ", "_");
        }

        private Mode GetMode(bool autoSave)
        {
            if (IsComMode)
                return Mode.Com;

            return autoSave ? Mode.AutoSave : Mode.Interactive;
        }

        private PdfCreatorUsageStatisticsMetric CreateJobUsageStatisticsMetric(Job job, TimeSpan duration, string status)
        {
            var pdfCreatorUsageStatisticsMetric = new PdfCreatorUsageStatisticsMetric
            {
                Product = GetApplicationName(),
                MachineId = _machineIdGenerator.GetMachineId(),
                OperatingSystem = _osHelper.GetWindowsVersion(),
                OutputFormat = job.Profile.OutputFormat.ToString(),
                Mode = GetMode(job.Profile.AutoSave.Enabled),
                QuickActions = job.Profile.ShowQuickActions,

                OpenViewer = job.Profile.OpenViewer,
                OpenWithPdfArchitect = job.Profile.OpenWithPdfArchitect,
                TotalPages = job.JobInfo.TotalPages,
                NumberOfCopies = job.NumberOfCopies,

                Version = _versionHelper?.ApplicationVersion?.ToString(),
                Duration = (long)duration.TotalMilliseconds,

                Dropbox = job.Profile.DropboxSettings.Enabled,
                Ftp = job.Profile.Ftp.Enabled,
                Smtp = job.Profile.EmailSmtpSettings.Enabled,
                Mailclient = job.Profile.EmailClientSettings.Enabled,
                Http = job.Profile.HttpSettings.Enabled,
                Print = job.Profile.Printing.Enabled,

                Cover = job.Profile.CoverPage.Enabled,
                Background = job.Profile.BackgroundPage.Enabled,
                Attachment = job.Profile.AttachmentPage.Enabled,
                Stamp = job.Profile.Stamping.Enabled,

                Encryption = job.Profile.PdfSettings.Security.Enabled,
                Signature = job.Profile.PdfSettings.Signature.Enabled,
                DisplaySignatureInDocument = job.Profile.PdfSettings.Signature.Enabled
                                             && job.Profile.PdfSettings.Signature.DisplaySignatureInDocument,

                Script = job.Profile.Scripting.Enabled,
                CustomScript = job.Profile.CustomScript.Enabled,
                UserToken = job.Profile.UserTokens.Enabled,
                IsShared = job.Profile.Properties.IsShared,

                DisableApplicationSettings = _gpoSettings.DisableApplicationSettings,
                DisableDebugTab = _gpoSettings.DisableDebugTab,
                DisablePrinterTab = _gpoSettings.DisablePrinterTab,
                DisableProfileManagement = _gpoSettings.DisableProfileManagement,
                DisableTitleTab = _gpoSettings.DisableTitleTab,
                DisableAccountsTab = _gpoSettings.DisableAccountsTab,
                DisableRssFeed = _gpoSettings.DisableRssFeed,
                DisableTips = _gpoSettings.DisableTips,
                HideLicenseTab = _gpoSettings.HideLicenseTab,
                HidePdfArchitectInfo = _gpoSettings.HidePdfArchitectInfo,
                GpoLanguage = _gpoSettings.Language ?? "",
                GpoUpdateInterval = _gpoSettings.UpdateInterval ?? "",
                LoadSharedAppSettings = _gpoSettings.LoadSharedAppSettings,
                LoadSharedProfiles = _gpoSettings.LoadSharedProfiles,
                AllowUserDefinedProfiles = _gpoSettings.AllowUserDefinedProfiles,

                Status = status
            };

            return pdfCreatorUsageStatisticsMetric;
        }
    }
}
