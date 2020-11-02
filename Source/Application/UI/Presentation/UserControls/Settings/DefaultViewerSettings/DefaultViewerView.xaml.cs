using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings
{
    public partial class DefaultViewerView : UserControl
    {
        public DefaultViewerView(DefaultViewerViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();

        }
    }
}
