using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public partial class SignatureUserControl : UserControl
    {
        public SignatureUserControl(SignatureUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);

            InitializeComponent();
        }
    }
}
