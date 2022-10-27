using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;
using UserControl = System.Windows.Controls.UserControl;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Attachment
{
    public partial class AttachmentActionView : UserControl, IRegionMemberLifetime, IActionView
    {
        public bool KeepAlive { get; } = true;

        public AttachmentActionView(AttachmentActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
