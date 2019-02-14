using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class ProfilesView : UserControl
    {
        public ProfilesView(ProfilesViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }
    }
}
