using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint
{
    public partial class UpdateHintView : UserControl
    {
        public UpdateHintView(UpdateHintViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
