using System.Windows;
using pdfforge.PDFCreator.UI.Presentation.Helper;


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

        private void ManagePrintJobsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            AccessKeyHelper.SetAccessKeys(sender);
        }
    }
}
