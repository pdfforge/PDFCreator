using System.Windows.Controls;


namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion
{
    /// <summary>
    /// Interaction logic for PageSizeView.xaml
    /// </summary>
    public partial class DirectConvertView : UserControl
    {
        public DirectConvertView(DirectConvertViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
