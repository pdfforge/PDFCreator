using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public class PdfCreatorUsageStatisticsViewModel : UsageStatisticsViewModelBase
    {
        private readonly IUsageMetricFactory _usageMetricFactory;
        public override bool ShowServiceSample => false;

        public PdfCreatorUsageStatisticsViewModel(IOsHelper osHelper, ICommandLocator commandLocator,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, IUsageMetricFactory usageMetricFactory,
            ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider)
            : base(osHelper, currentSettingsProvider, gpoSettings, translationUpdater, usageStatisticsProvider, commandLocator, applicationNameProvider)
        {
            _usageMetricFactory = usageMetricFactory;
        }


        public override HelpTopic HelpTopic => HelpTopic.AppGeneral;
        public override bool IsDisabledByGpo => GpoSettings.DisableUsageStatistics;

        protected override string GetJobSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<PdfCreatorJobFinishedMetric>();
            
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

        protected override string GetServiceSampleData()
        {
            return "";
        }
    }
}
