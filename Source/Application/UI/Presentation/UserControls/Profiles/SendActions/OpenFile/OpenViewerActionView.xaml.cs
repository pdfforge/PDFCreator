using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile
{
    /// <summary>
    /// Interaction logic for OpenFileUserControl.xaml
    /// </summary>
    public partial class OpenViewerActionView : UserControl, IActionView
    {
        public OpenViewerActionView(OpenViewerActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
