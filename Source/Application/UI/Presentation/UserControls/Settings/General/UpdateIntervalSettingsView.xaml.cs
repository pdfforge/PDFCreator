using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    /// <summary>
    ///     Interaction logic for UpdateIntervalSettingsView.xaml
    /// </summary>
    public partial class UpdateIntervalSettingsView : UserControl
    {
        public UpdateIntervalSettingsView()
        {
            InitializeComponent();
        }

        public void SetDataContext(UpdateIntervalSettingsViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
