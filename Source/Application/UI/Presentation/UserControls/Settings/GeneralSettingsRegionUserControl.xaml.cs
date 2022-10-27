using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    /// <summary>
    /// Interaction logic for GeneralSettingsView.xaml
    /// </summary>
    public partial class GeneralSettingsView : UserControl
    {

        public GeneralSettingsView(
            LanguageSelectionSettingsViewModel languageSelectionSettingsViewModel,
            UpdateIntervalSettingsViewModel updateIntervalSettingsViewModel,
            DefaultPrinterSettingsViewModel defaultPrinterSettingsViewModel,
            HomeViewSettingsViewModel homeViewSettingsViewModel,
            HotStandbySettingsViewModel hotStandbySettingsViewModel,
            ExplorerIntegrationSettingsViewModel explorerIntegrationSettingsViewModel,
            UsageStatisticsViewModelBase usageStatisticsViewModel,
            GeneralSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            LanguageSelectionSettingsView.SetDataContext(languageSelectionSettingsViewModel);
            UpdateIntervalSettingsView.SetDataContext(updateIntervalSettingsViewModel);
            DefaultPrinterSettingsView.SetDataContext(defaultPrinterSettingsViewModel);
            HomeViewSettingsView.SetDataContext(homeViewSettingsViewModel);
            HotStandbySettingsView.SetDataContext(hotStandbySettingsViewModel);
            ExplorerIntegrationSettingsView.SetDataContext(explorerIntegrationSettingsViewModel);
            UsageStatisticsView.SetDataContext(usageStatisticsViewModel);
            TransposerHelper.Register(this, viewModel);

        }
    }
}
