using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class BusinessHintStatusBarControl : UserControl
    {
        public BusinessHintStatusBarControl(BusinessHintStatusBarViewModel businessHintStatusBarViewModel)
        {
            DataContext = businessHintStatusBarViewModel;
            InitializeComponent();
        }
    }
}
