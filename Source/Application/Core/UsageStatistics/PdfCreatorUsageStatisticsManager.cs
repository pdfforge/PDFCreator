using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.UsageStatistics;
using System;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class PdfCreatorUsageStatisticsManager : IPdfCreatorUsageStatisticsManager
    {
        private readonly IUsageStatisticsSender _sender;
        private readonly IOsHelper _osHelper;
        private readonly IUsageMetricFactory _usageMetricFactory;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly IThreadManager _threadManager;

        public PdfCreatorUsageStatisticsManager(IUsageStatisticsSender sender, IOsHelper osHelper,
            IUsageMetricFactory usageMetricFactory,
            ISettingsProvider settingsProvider, IGpoSettings gpoSettings, IThreadManager threadManager)
        {
            _sender = sender;
            _osHelper = osHelper;
            _usageMetricFactory = usageMetricFactory;
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

        private Mode GetMode(bool autoSave)
        {
            if (IsComMode)
                return Mode.Com;

            return autoSave ? Mode.AutoSave : Mode.Interactive;
        }

        private PdfCreatorJobFinishedMetric CreateJobUsageStatisticsMetric(Job job, TimeSpan duration, string status)
        {
            var metric = _usageMetricFactory.CreateMetric<PdfCreatorJobFinishedMetric>();
            
            metric.OperatingSystem = _osHelper.GetWindowsVersion();

            //Job
            metric.OutputFormat = job.Profile.OutputFormat.ToString();
            metric.Mode = GetMode(job.Profile.AutoSave.Enabled);
            metric.SaveFileTemporarily = job.Profile.SaveFileTemporary;
            metric.AutoSaveExistingFileBehaviour = job.Profile.AutoSave.ExistingFileBehaviour.ToString();
            metric.SkipSendFailures = job.Profile.SkipSendFailures;
            metric.QuickActions = job.Profile.ShowQuickActions;
            metric.ShowTrayNotifications = job.Profile.ShowAllNotifications;

            //Result
            metric.TotalPages = job.JobInfo.TotalPages;
            metric.NumberOfCopies = job.NumberOfCopies;
            metric.Duration = (long)duration.TotalMilliseconds;
            metric.Status = status;

            //Preparation Actions
            metric.CustomScript = job.Profile.CustomScript.Enabled;
            metric.UserToken = job.Profile.UserTokens.Enabled;
            metric.ForwardToFurtherProfile = job.Profile.ForwardToFurtherProfile.Enabled;

            //Modify Actions
            metric.Cover = job.Profile.CoverPage.Enabled;
            metric.Attachment = job.Profile.AttachmentPage.Enabled;
            metric.Stamp = job.Profile.Stamping.Enabled;
            metric.Watermark = job.Profile.Watermark.Enabled;
            metric.Background = job.Profile.BackgroundPage.Enabled;
            metric.PageNumbers = job.Profile.PageNumbers.Enabled;
            metric.Encryption = job.Profile.PdfSettings.Security.Enabled;
            metric.Signature = job.Profile.PdfSettings.Signature.Enabled;
            metric.DisplaySignatureInDocument = job.Profile.PdfSettings.Signature.Enabled
                                             && job.Profile.PdfSettings.Signature.DisplaySignature != DisplaySignature.NoDisplay;

            //Send actions
            metric.Mailclient = job.Profile.EmailClientSettings.Enabled;
            metric.MailWebAccess = job.Profile.EmailWebSettings.Enabled;
            metric.Smtp = job.Profile.EmailSmtpSettings.Enabled;
            metric.OpenViewer = job.Profile.OpenViewer.Enabled;
            metric.OpenWithPdfArchitect = job.Profile.OpenViewer.OpenWithPdfArchitect;
            metric.Script = job.Profile.Scripting.Enabled;
            metric.Print = job.Profile.Printing.Enabled;
            metric.Ftp = job.Profile.Ftp.Enabled;
            metric.Http = job.Profile.HttpSettings.Enabled;
            metric.Dropbox = job.Profile.DropboxSettings.Enabled;
            
            //GPOs
            metric.DisableApplicationSettings = _gpoSettings.DisableApplicationSettings;
            metric.DisableDebugTab = _gpoSettings.DisableDebugTab;
            metric.DisablePrinterTab = _gpoSettings.DisablePrinterTab;
            metric.DisableProfileManagement = _gpoSettings.DisableProfileManagement;
            metric.DisableTitleTab = _gpoSettings.DisableTitleTab;
            metric.DisableHistory = _gpoSettings.DisableHistory;
            metric.DisableAccountsTab = _gpoSettings.DisableAccountsTab;
            metric.DisableRssFeed = _gpoSettings.DisableRssFeed;
            metric.DisableTips = _gpoSettings.DisableTips;
            metric.HideLicenseTab = _gpoSettings.HideLicenseTab;
            metric.HidePdfArchitectInfo = _gpoSettings.HidePdfArchitectInfo;
            metric.GpoLanguage = _gpoSettings.Language ?? "";
            metric.DisableLicenseExpirationReminder = _gpoSettings.DisableLicenseExpirationReminder;
            metric.GpoUpdateInterval = _gpoSettings.UpdateInterval ?? "";
            metric.GpoHotStandbyMinutes = _gpoSettings.HotStandbyMinutes;

            //Share settings
            metric.LoadSharedAppSettings = _gpoSettings.LoadSharedAppSettings;
            metric.LoadSharedProfiles = _gpoSettings.LoadSharedProfiles;
            metric.IsShared = job.Profile.Properties.IsShared;
            metric.AllowUserDefinedProfiles = _gpoSettings.AllowUserDefinedProfiles;
            metric.HasShareFilename = _gpoSettings.SharedSettingsFilename != "settings";

            return metric;
        }
    }
}
