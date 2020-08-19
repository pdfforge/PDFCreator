using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP
{
    public partial class HttpActionUserControl : UserControl, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
        
        public HttpActionUserControl(HttpActionViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

    }
}
