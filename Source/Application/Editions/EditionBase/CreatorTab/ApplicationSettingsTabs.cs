using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings;
using pdfforge.PDFCreator.UI.PrismHelper.Tab;

namespace pdfforge.PDFCreator.Editions.EditionBase.CreatorTab
{
    public class ApplicationSettingsTabs : TabRegion, IApplicationSettingsTabs
    {
        public ApplicationSettingsTabs() : base(RegionNames.SettingsTabRegion)
        {
            SetupTabs();
        }

        protected virtual void SetupTabs()
        {
            Add(new MultiTab<GeneralSettingsViewModel>(RegionNames.GeneralSettingsTabContentRegion, HelpTopic.AppGeneral, typeof(LanguageSelectionSettingsView), typeof(UpdateIntervalSettingsView), typeof(DefaultPrinterSettingsView), typeof(HomeViewSettingsView), typeof(ExplorerIntegrationSettingsView), typeof(UsageStatisticsView)));
            Add(new SimpleTab<TitleReplacementsView, TitleReplacementsViewModel>(RegionNames.TitleReplacementTabContentRegion, HelpTopic.AppTitle));
            Add(new SimpleTab<DefaultViewerView, DefaultViewerViewModel>(RegionNames.DefaultViewerTabContentRegion, HelpTopic.AppViewer));
            Add(new MultiTab<DebugSettingsViewModel>(RegionNames.DebugSettingsTabContentRegion, HelpTopic.AppDebug, typeof(LoggingSettingView), typeof(TestPageSettingsView), typeof(RestoreSettingsView), typeof(ExportSettingView)));
        }
    }

    public interface IApplicationSettingsTabs : ITabRegion
    {
    }

    public class ApplicationSettingsWithoutUpdateIntervalTabs : ApplicationSettingsTabs
    {
        protected override void SetupTabs()
        {
            Add(new MultiTab<GeneralSettingsViewModel>(RegionNames.GeneralSettingsTabContentRegion, HelpTopic.AppGeneral, typeof(LanguageSelectionSettingsView), typeof(DefaultPrinterSettingsView), typeof(HomeViewSettingsView), typeof(ExplorerIntegrationSettingsView), typeof(UsageStatisticsView)));
            Add(new SimpleTab<TitleReplacementsView, TitleReplacementsViewModel>(RegionNames.TitleReplacementTabContentRegion, HelpTopic.AppTitle));
            Add(new SimpleTab<DefaultViewerView, DefaultViewerViewModel>(RegionNames.DefaultViewerTabContentRegion, HelpTopic.AppViewer));
            Add(new MultiTab<DebugSettingsViewModel>(RegionNames.DebugSettingsTabContentRegion, HelpTopic.AppDebug, typeof(LoggingSettingView), typeof(TestPageSettingsView), typeof(RestoreSettingsView), typeof(ExportSettingView)));
        }
    }
}
