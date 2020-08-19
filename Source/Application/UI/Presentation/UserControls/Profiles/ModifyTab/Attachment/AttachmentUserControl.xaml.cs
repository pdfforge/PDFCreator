using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using UserControl = System.Windows.Controls.UserControl;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public partial class AttachmentUserControl : UserControl, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;

        public AttachmentUserControl(AttachmentUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
