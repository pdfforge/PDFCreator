using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile
{
    /// <summary>
    /// Interaction logic for OpenFileUserControl.xaml
    /// </summary>
    public partial class OpenViewerActionUserControl : UserControl, IActionUserControl
    {
        public OpenViewerActionUserControl(OpenViewerActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
