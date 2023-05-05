using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    /// <summary>
    ///     Interaction logic for DefaultPrinterSettingsView.xaml
    /// </summary>
    public partial class DefaultPrinterSettingsView : UserControl
    {
        public DefaultPrinterSettingsView(DefaultPrinterSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
