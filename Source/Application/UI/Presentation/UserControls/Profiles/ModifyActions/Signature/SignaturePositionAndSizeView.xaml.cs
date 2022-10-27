using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    /// <summary>
    /// Interaction logic for SignaturePositionAndSizeView.xaml
    /// </summary>
    public partial class SignaturePositionAndSizeView : UserControl
    {
        public SignaturePositionAndSizeView(SignaturePositionAndSizeViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
