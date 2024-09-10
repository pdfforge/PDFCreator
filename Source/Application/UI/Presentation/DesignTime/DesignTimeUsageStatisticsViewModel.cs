using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUsageStatisticsViewModel : UsageStatisticsViewModelBase
    {
        public DesignTimeUsageStatisticsViewModel() : base(new OsHelper(),
            new DesignTimeCurrentSettingsProvider(), new GpoSettingsDefaults(), new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettings<Conversion.Settings.UsageStatistics>(),
            new DesignTimeCommandLocator(), new DesignTimeApplicationNameProvider(), new DesignTimeUsageStatisticsJsonSerializer())
        { }

        public override HelpTopic HelpTopic => HelpTopic.General;
        public override bool IsDisabledByGpo => false;
        public override bool ShowServiceSample => false;

        protected override string GetJobSampleData() => "{ }";

        protected override string GetServiceSampleData() => "{ }";
    }
}
