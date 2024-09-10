using System.Windows.Controls;
using MdXaml;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint
{
    public partial class UpdateHintView : UserControl
    {
        public UpdateHintView(UpdateHintViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            // Required to prevent ms build to remove the MdXaml assembly from the output
            var _ = typeof(Markdown);
        }
    }
}
