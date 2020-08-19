using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class AddActionOverlayView : UserControl
    {
        public AddActionOverlayView(AddActionOverlayViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
