using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript
{
    /// <summary>
    /// Interaction logic for CsScriptView.xaml
    /// </summary>
    public partial class CsScriptUserControl : UserControl, IActionUserControl
    {
        public IActionViewModel ViewModel { get; }

        public CsScriptUserControl(CsScriptViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
