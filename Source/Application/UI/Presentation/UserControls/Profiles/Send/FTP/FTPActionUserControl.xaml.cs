using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP
{
    public partial class FTPActionUserControl : UserControl, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
        public FTPActionUserControl(FtpActionViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
