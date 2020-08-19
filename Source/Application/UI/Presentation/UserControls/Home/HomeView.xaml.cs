using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public partial class HomeView : UserControl
    {
        public HomeView(HomeViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            TransposerHelper.Register(this, vm);
        }
    }
}
