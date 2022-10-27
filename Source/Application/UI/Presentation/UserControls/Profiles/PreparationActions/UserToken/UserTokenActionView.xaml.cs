using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken
{
    public partial class UserTokenActionView : UserControl, IRegionMemberLifetime, IActionView
    {
        public bool KeepAlive { get; } = true;

        public UserTokenActionView(UserTokenActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
