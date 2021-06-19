using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp
{
    public partial class EditEmailTextUserControl : IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;

        public EditEmailTextUserControl(EditEmailTextViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
