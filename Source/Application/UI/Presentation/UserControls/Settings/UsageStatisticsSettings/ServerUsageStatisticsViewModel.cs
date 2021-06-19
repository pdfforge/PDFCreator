using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public class ServerUsageStatisticsViewModel : UsageStatisticsViewModelBase
    {
        private readonly IUsageMetricFactory _usageMetricFactory;

        public override bool ShowServiceSample => true;



        public ServerUsageStatisticsViewModel(IOsHelper osHelper, ICommandLocator commandLocator,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, IUsageMetricFactory usageMetricFactory,
            ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider)
            : base(osHelper, currentSettingsProvider, gpoSettings, translationUpdater, usageStatisticsProvider, commandLocator, applicationNameProvider)
        {
            _usageMetricFactory = usageMetricFactory;
        }


        protected override string GetJobSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<ServerJobFinishedMetric>();
            
            metric.Duration = TimeSpan.Zero.Milliseconds;
            metric.OutputFormat = OutputFormat.Pdf.ToString();
            metric.Status = "Success";
            metric.Attachment = true;
            metric.Background = true;
            metric.Dropbox = true;
            metric.Cover = true;
            metric.NumberOfCopies = 1;
            metric.Script = true;
            metric.CustomScript = true;
            metric.TotalPages = 1;
            metric.Print = true;
            metric.Signature = true;
            metric.Encryption = true;
            metric.UserToken = true;
            metric.Ftp = true;
            metric.Http = true;
            metric.Smtp = true;
            metric.Stamp = true;

            return ConvertToJson(metric);
        }

        protected override string GetServiceSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<ServiceStoppedMetric>();
            metric.ServiceUptime = TimeSpan.TicksPerMillisecond;
            metric.TotalUsers = 1;
            metric.TotalDocuments = 1;
            metric.OperatingSystem = OsHelper.GetWindowsVersion();

            return ConvertToJson(metric);
        }

        public override HelpTopic HelpTopic => HelpTopic.ServerGeneralSettingsTab;
        public override bool IsDisabledByGpo { get; }
    }
}
