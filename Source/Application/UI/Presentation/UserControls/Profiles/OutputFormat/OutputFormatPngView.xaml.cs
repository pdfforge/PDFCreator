using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class OutputFormatPngView : UserControl
    {
        public OutputFormatPngView(OutputFormatViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
