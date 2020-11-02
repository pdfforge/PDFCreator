using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.OpenFile
{
    /// <summary>
    /// Interaction logic for OpenFileUserControl.xaml
    /// </summary>
    public partial class OpenViewerActionUserControl : UserControl
    {
        public OpenViewerActionUserControl(OpenViewerActionViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
