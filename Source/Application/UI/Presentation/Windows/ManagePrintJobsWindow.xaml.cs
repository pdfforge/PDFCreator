using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public partial class ManagePrintJobsWindow
    {
        public ManagePrintJobsWindow(ManagePrintJobsViewModel viewModel, IHightlightColorRegistration hightlightColorRegistration)
        {
            DataContext = viewModel;
            InitializeComponent();
            hightlightColorRegistration.RegisterHighlightColorResource(this);

            // dummy reference to force GongSolutions.Wpf.DragDrop to be copied into bin folder
            var t = typeof(GongSolutions.Wpf.DragDrop.DragDrop);
        }
    }
}
