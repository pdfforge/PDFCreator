namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public partial class ManagePrintJobsWindow
    {
        public ManagePrintJobsWindow(ManagePrintJobsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            // dummy reference to force GongSolutions.Wpf.DragDrop to be copied into bin folder
            var t = typeof(GongSolutions.Wpf.DragDrop.DragDrop);
        }
    }
}
