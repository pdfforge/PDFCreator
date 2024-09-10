using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft
{
    /// <summary>
    /// Interaction logic for MicrosoftAccountView.xaml
    /// </summary>
    public partial class MicrosoftAccountView : UserControl
    {
        public MicrosoftAccountView(MicrosoftAccountViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }
    }
}
