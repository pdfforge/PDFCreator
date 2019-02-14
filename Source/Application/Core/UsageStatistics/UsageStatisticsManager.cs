using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class UsageStatisticsManager : IUsageStatisticsManager
    {
        private readonly IUsageStatisticsSender _sender;
        private readonly IOsHelper _osHelper;
        private readonly IMachineIdGenerator _machineIdGenerator;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;
        private int _processedJobCounter;
        private readonly ISet<string> _activeUsers = new HashSet<string>();

        public bool EnableUsageStatistics { get; set; }

        public UsageStatisticsManager(IUsageStatisticsSender sender, IOsHelper osHelper, IMachineIdGenerator machineIdGenerator,
                                      ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper)
        {
            _sender = sender;
            _osHelper = osHelper;
            _machineIdGenerator = machineIdGenerator;
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
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

        private string GetApplicationName()
        {
            return _applicationNameProvider.ApplicationNameWithEdition.ToLower().Replace(" ", "_");
        }

        private JobUsageStatisticsMetric CreateJobUsageStatisticsMetric(Job job, TimeSpan duration, string status)
        {
            var usageStatisticsMetric = new JobUsageStatisticsMetric
            {
                Product = GetApplicationName(),
                MachineId = _machineIdGenerator.GetMachineId(),
                OutputFormat = job.Profile.OutputFormat.ToString(),

                TotalPages = job.JobInfo.TotalPages,
                NumberOfCopies = job.NumberOfCopies,

                Version = _versionHelper?.ApplicationVersion?.ToString(),
                Duration = (long)duration.TotalMilliseconds,

                Dropbox = job.Profile.DropboxSettings.Enabled,
                Ftp = job.Profile.Ftp.Enabled,
                Smtp = job.Profile.EmailSmtpSettings.Enabled,
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

                Status = status
            };

            return usageStatisticsMetric;
        }

        private ServiceUsageStatisticsMetric SerializeServiceUsageStatistics(TimeSpan serviceUptime)
        {
            var serviceUsageStatisticsMetric = new ServiceUsageStatisticsMetric
            {
                Product = GetApplicationName(),
                Version = _versionHelper?.ApplicationVersion?.ToString(),
                MachineId = _machineIdGenerator.GetMachineId(),
                TotalDocuments = _processedJobCounter,
                TotalUsers = _activeUsers.Count,
                OperatingSystem = _osHelper.GetWindowsVersion(),
                ServiceUptime = (long)serviceUptime.TotalMilliseconds
            };

            return serviceUsageStatisticsMetric;
        }
    }
}
