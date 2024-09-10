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
            ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, ITranslationUpdater translationUpdater, 
            ApplicationNameProvider applicationNameProvider, IUsageStatisticsJsonSerializer usageStatisticsJsonSerializer)
            : base(osHelper, currentSettingsProvider, gpoSettings, translationUpdater, usageStatisticsProvider, commandLocator, 
                applicationNameProvider, usageStatisticsJsonSerializer)
        {
            _usageMetricFactory = usageMetricFactory;
        }


        public override HelpTopic HelpTopic => HelpTopic.AppGeneral;
        public override bool IsDisabledByGpo => GpoSettings.DisableUsageStatistics;

        protected override string GetJobSampleData()
        {
            var metric = _usageMetricFactory.CreateMetric<PdfCreatorJobFinishedMetric>();

            metric.OperatingSystem = OsHelper.GetWindowsVersion();
            metric.OutputFormat = OutputFormat.Pdf.ToString();
            metric.Mode = Mode.Interactive;
            metric.TotalPages = 1;
            metric.NumberOfCopies = 1;
            metric.Duration = TimeSpan.Zero.Milliseconds;
            metric.Status = "Success";

            return UsageStatisticsJsonSerializer.Serialize(metric);
        }

        protected override string GetServiceSampleData()
        {
            return "";
        }
    }
}
