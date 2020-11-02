using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared
{
    /// <summary>
    ///     Interaction logic for SettingControlsView.xaml
    /// </summary>
    public partial class SaveCancelButtonsControl : UserControl
    {
        public SaveCancelButtonsControl(SettingControlsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
