using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public partial class SignatureActionView : UserControl, IRegionMemberLifetime, IActionView
    {
        public bool KeepAlive { get; } = true;

        public SignatureActionView(SignatureActionViewModel actionViewModel)
        {
            DataContext = actionViewModel;
            ViewModel = actionViewModel;
            TransposerHelper.Register(this, actionViewModel);

            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
