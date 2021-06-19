using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    /// <summary>
    /// Interaction logic for SaveOutputformatMetadataView.xaml
    /// </summary>
    public partial class SaveOutputFormatMetadataView : UserControl
    {
        public SaveOutputFormatMetadataView(SaveOutputFormatMetadataViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
