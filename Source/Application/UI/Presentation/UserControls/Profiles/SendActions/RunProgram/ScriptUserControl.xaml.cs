using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.RunProgram
{
    public partial class ScriptUserControl : UserControl, IRegionMemberLifetime, IActionUserControl
    {
        public bool KeepAlive { get; } = true;

        public IActionViewModel ViewModel { get; }

        public ScriptUserControl(ScriptUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
