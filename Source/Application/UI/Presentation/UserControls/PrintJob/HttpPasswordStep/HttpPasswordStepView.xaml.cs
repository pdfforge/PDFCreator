namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for HttpPasswordView.xaml
    /// </summary>
    public partial class HttpPasswordStepView : System.Windows.Controls.UserControl
    {
        public HttpPasswordStepView(HttpPasswordStepViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
