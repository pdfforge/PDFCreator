using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class OutputFormatView : UserControl
    {
        public OutputFormatView(OutputFormatViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
