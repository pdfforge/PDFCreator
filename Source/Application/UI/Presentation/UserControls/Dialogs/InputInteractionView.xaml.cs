namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Dialogs
{
    public partial class InputInteractionView
    {
        public InputInteractionView(InputBoxWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
