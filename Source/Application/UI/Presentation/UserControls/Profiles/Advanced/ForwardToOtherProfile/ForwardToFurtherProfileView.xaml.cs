using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.ForwardToOtherProfile
{
    /// <summary>
    /// Interaction logic for ForwardToOtherProfileView.xaml
    /// </summary>
    public partial class ForwardToFurtherProfileView : UserControl
    {
        public ForwardToFurtherProfileView(ForwardToFurtherProfileViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
