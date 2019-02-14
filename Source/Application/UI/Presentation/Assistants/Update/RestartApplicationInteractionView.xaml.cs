using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public partial class RestartApplicationInteractionView : UserControl
    {
        public RestartApplicationInteractionView(RestartApplicationInteractionViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
