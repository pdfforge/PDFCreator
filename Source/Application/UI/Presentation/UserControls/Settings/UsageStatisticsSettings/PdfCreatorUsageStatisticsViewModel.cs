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
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public class PdfCreatorUsageStatisticsViewModel : UsageStatisticsViewModelBase
    {
        private readonly IUsageMetricFactory _usageMetricFactory;
        public string SampleStatisticsJobData => GetJobSampleData();
        public bool ShowServiceSample => false;
        public bool EnableUsageStatistics
        {
            get
            {
                return UsageStatisticsProvider.Settings.Enable && !IsDisabledByGpo;
            }
            set
            {
                UsageStatisticsProvider.Settings.Enable = value;
                RaisePropertyChanged(nameof(EnableUsageStatistics));
            }
        }


        public PdfCreatorUsageStatisticsViewModel(IOsHelper osHelper, ICommandLocator commandLocator,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, IUsageMetricFactory usageMetricFactory,
            ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider)
            : base(osHelper, currentSettingsProvider, gpoSettings, translationUpdater, usageStatisticsProvider, commandLocator, applicationNameProvider)
        {
            _usageMetricFactory = usageMetricFactory;
        }

        public string UsageStatisticsExplanationText => Translation.FormatUsageStatisticsExplanationText(base.ApplicationNameWithEdition);

        public override HelpTopic HelpTopic => HelpTopic.AppGeneral;
        public override bool IsDisabledByGpo => GpoSettings.DisableUsageStatistics;

        private string GetJobSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<PdfCreatorUsageStatisticsMetric>();
            
            metric.OperatingSystem = OsHelper.GetWindowsVersion();
            metric.Duration = TimeSpan.Zero.Milliseconds;
            metric.OutputFormat = OutputFormat.Pdf.ToString();
            metric.Mode = Mode.Interactive;
            metric.QuickActions = true;
            metric.OpenViewer = true;
            metric.OpenWithPdfArchitect = true;
            metric.Status = "Success";
            metric.Attachment = true;
            metric.Background = true;
            metric.Dropbox = true;
            metric.Cover = true;
            metric.NumberOfCopies = 1;
            metric.Script = true;
            metric.CustomScript = true;
            metric.TotalPages = 1;
            metric.Mailclient = true;
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

    }

    public class DesignTimePdfCreatorUsageStatisticsViewModel : PdfCreatorUsageStatisticsViewModel
    {
        public DesignTimePdfCreatorUsageStatisticsViewModel() : base(new OsHelper(), new DesignTimeCommandLocator(),
                                                            new DesignTimeCurrentSettingsProvider(), new GpoSettingsDefaults(), new DesignTimeUsageMetricFactory(), 
                                                            new DesignTimeCurrentSettings<Conversion.Settings.UsageStatistics>(), new DesignTimeTranslationUpdater(), new DesignTimeApplicationNameProvider())
        {

        }
    }
}
