using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public partial class RssFeedView : UserControl
    {
        public RssFeedView(RssFeedViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }
    }
}
