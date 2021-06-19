using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities;
using pdfforge.UsageStatistics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class UsageStatisticsManager : IUsageStatisticsManager
    {
        private readonly IUsageStatisticsSender _sender;
        private readonly IUsageMetricFactory _usageMetricFactory;
        private readonly IOsHelper _osHelper;
        private int _processedJobCounter;
        private readonly ISet<string> _activeUsers = new HashSet<string>();

        public bool EnableUsageStatistics { get; set; }

        public UsageStatisticsManager(IUsageStatisticsSender sender, IUsageMetricFactory usageMetricFactory, IOsHelper osHelper)
        {
            _sender = sender;
            _usageMetricFactory = usageMetricFactory;
            _osHelper = osHelper;
            _processedJobCounter = 0;
        }

        public async Task SendUsageStatistics(TimeSpan duration, Job job, string status)
        {
            if (EnableUsageStatistics)
            {
                var usageMetric = CreateJobUsageStatisticsMetric(job, duration, status);

                await _sender.SendAsync(usageMetric);

                IncreaseGlobalCounters(job.JobInfo.Metadata.Author);
            }
        }

        private void IncreaseGlobalCounters(string username)
        {
            _processedJobCounter++;
            _activeUsers.Add(username);
        }

        private void ResetGlobalCounters()
        {
            _processedJobCounter = 0;
            _activeUsers.Clear();
        }

        public async Task SendServiceStatistics(TimeSpan serviceUptime)
        {
            if (EnableUsageStatistics)
            {
                var usageMetric = SerializeServiceUsageStatistics(serviceUptime);
                await _sender.SendAsync(usageMetric);

                ResetGlobalCounters();
            }
        }

        private ServerJobFinishedMetric CreateJobUsageStatisticsMetric(Job job, TimeSpan duration, string status)
        {
            var metric = _usageMetricFactory.CreateMetric<ServerJobFinishedMetric>();
            metric.OutputFormat = job.Profile.OutputFormat.ToString();

            metric.TotalPages = job.JobInfo.TotalPages;
            metric.NumberOfCopies = job.NumberOfCopies;

            metric.Duration = (long)duration.TotalMilliseconds;

            metric.Dropbox = job.Profile.DropboxSettings.Enabled;
            metric.Ftp = job.Profile.Ftp.Enabled;
            metric.Smtp = job.Profile.EmailSmtpSettings.Enabled;
            metric.Http = job.Profile.HttpSettings.Enabled;
            metric.Print = job.Profile.Printing.Enabled;

            metric.Cover = job.Profile.CoverPage.Enabled;
            metric.Background = job.Profile.BackgroundPage.Enabled;
            metric.Attachment = job.Profile.AttachmentPage.Enabled;
            metric.Stamp = job.Profile.Stamping.Enabled;

            metric.Encryption = job.Profile.PdfSettings.Security.Enabled;
            metric.Signature = job.Profile.PdfSettings.Signature.Enabled;
            metric.DisplaySignatureInDocument = job.Profile.PdfSettings.Signature.Enabled
                                                && job.Profile.PdfSettings.Signature.DisplaySignatureInDocument;
            metric.Script = job.Profile.Scripting.Enabled;
            metric.CustomScript = job.Profile.CustomScript.Enabled;
            metric.UserToken = job.Profile.UserTokens.Enabled;

            metric.Status = status;

            return metric;
        }

        private ServiceStoppedMetric SerializeServiceUsageStatistics(TimeSpan serviceUptime)
        {
            var metric = _usageMetricFactory.CreateMetric<ServiceStoppedMetric>();
            metric.TotalDocuments = _processedJobCounter;
            metric.TotalUsers = _activeUsers.Count;
            metric.OperatingSystem = _osHelper.GetWindowsVersion();
            metric.ServiceUptime = (long)serviceUptime.TotalMilliseconds;

            return metric;
        }
    }
}
