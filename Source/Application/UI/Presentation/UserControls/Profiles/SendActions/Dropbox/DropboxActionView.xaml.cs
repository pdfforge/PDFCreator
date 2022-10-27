using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Dropbox
{
    public partial class DropboxActionView : UserControl, IActionView
    {
        public DropboxActionView(DropboxActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
