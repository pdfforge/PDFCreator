using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows;
using System.Windows.Controls;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public partial class SignatureUserControl : UserControl, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
        public SignatureUserControl(SignatureUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);

            InitializeComponent();
        }
    }
}
