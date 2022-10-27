using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class SaveView : UserControl
    {
        public SaveView(SaveViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }
    }
}
