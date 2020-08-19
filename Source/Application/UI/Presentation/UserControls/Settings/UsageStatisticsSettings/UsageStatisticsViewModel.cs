using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public class UsageStatisticsViewModel : UsageStatisticsViewModelBase
    {
        private readonly IUsageMetricFactory _usageMetricFactory;

        public string SampleStatisticsJobData => GetJobSampleData();
        public string SampleStatisticsData => GetServiceSampleData();
        public bool ShowServiceSample => true;
        public bool EnableUsageStatistics
        {
            get
            {
                return UsageStatisticsProvider.Settings.Enable;
            }
            set
            {
                UsageStatisticsProvider.Settings.Enable = value;
                RaisePropertyChanged(nameof(EnableUsageStatistics));
            }
        }


        public UsageStatisticsViewModel(IOsHelper osHelper, ICommandLocator commandLocator,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, IUsageMetricFactory usageMetricFactory,
            ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider)
            : base(osHelper, currentSettingsProvider, gpoSettings, translationUpdater, usageStatisticsProvider, commandLocator, applicationNameProvider)
        {
            _usageMetricFactory = usageMetricFactory;
        }

        public string UsageStatisticsExplanationText => Translation.FormatUsageStatisticsExplanationText(base.ApplicationNameWithEdition);

        private string GetJobSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<JobUsageStatisticsMetric>();
            
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

        private string GetServiceSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<ServiceUsageStatisticsMetric>();
            metric.ServiceUptime = TimeSpan.TicksPerMillisecond;
            metric.TotalUsers = 1;
            metric.TotalDocuments = 1;
            metric.OperatingSystem = OsHelper.GetWindowsVersion();

            return ConvertToJson(metric);
        }

        public override HelpTopic HelpTopic => HelpTopic.ServerGeneralSettingsTab;
        public override bool IsDisabledByGpo { get; }
    }

    public class DesignTimeUsageStatisticsViewModel : UsageStatisticsViewModel
    {
        public DesignTimeUsageStatisticsViewModel() : base(new OsHelper(), new DesignTimeCommandLocator(),
                                                            new DesignTimeCurrentSettingsProvider(), new GpoSettingsDefaults(),new DesignTimeUsageMetricFactory(), 
                                                            new DesignTimeCurrentSettings<Conversion.Settings.UsageStatistics>(), new DesignTimeTranslationUpdater(), 
                                                            new DesignTimeApplicationNameProvider())
        {

        }
    }
}
