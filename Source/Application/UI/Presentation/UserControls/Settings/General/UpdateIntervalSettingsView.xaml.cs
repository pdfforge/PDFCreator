using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    /// <summary>
    ///     Interaction logic for UpdateIntervalSettingsView.xaml
    /// </summary>
    public partial class UpdateIntervalSettingsView : UserControl
    {
        public UpdateIntervalSettingsView(UpdateIntervalSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
