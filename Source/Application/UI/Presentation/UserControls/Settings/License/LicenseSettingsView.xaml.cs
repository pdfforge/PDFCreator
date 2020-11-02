using System.Windows.Forms;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using UserControl = System.Windows.Controls.UserControl;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    /// <summary>
    ///     Interaction logic for LicenseSettingsView.xaml
    /// </summary>
    ///
    [ViewSortHint("100")]
    public partial class LicenseSettingsView : UserControl
    {
        public LicenseSettingsViewModel ViewModel => (LicenseSettingsViewModel) DataContext;

        public LicenseSettingsView(LicenseSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
        }
    }
}
