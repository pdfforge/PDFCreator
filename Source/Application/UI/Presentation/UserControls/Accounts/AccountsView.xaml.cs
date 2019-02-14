using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public partial class AccountsView : UserControl
    {
        public AccountsView(AccountsViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }
    }
}
