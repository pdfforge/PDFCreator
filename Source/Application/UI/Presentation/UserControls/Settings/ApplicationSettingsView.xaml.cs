using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    /// <summary>
    ///     Interaction logic for ApplicationSettingsView.xaml
    /// </summary>
    public partial class ApplicationSettingsView : UserControl
    {
        public ApplicationSettingsView(SettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            TransposerHelper.Register(this, viewModel);
        }
    }
}
