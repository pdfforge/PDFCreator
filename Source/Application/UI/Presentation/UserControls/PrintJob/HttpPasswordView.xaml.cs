namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for HttpPasswordView.xaml
    /// </summary>
    public partial class HttpPasswordView : System.Windows.Controls.UserControl
    {
        public HttpPasswordView(HttpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
