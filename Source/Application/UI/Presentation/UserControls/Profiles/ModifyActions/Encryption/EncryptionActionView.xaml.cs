using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Encryption
{
    public partial class EncryptionActionView : UserControl, IRegionMemberLifetime, IActionView
    {
        public bool KeepAlive { get; } = true;

        public EncryptionActionView(EncryptionActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
