using Microsoft.Xaml.Behaviors;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Feedback
{
    public partial class FeedbackSentView
    {
        public FeedbackSentView(FeedbackSentViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
