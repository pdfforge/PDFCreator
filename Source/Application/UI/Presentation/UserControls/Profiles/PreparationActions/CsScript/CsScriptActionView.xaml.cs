using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript
{
    /// <summary>
    /// Interaction logic for CsScriptView.xaml
    /// </summary>
    public partial class CsScriptActionView : UserControl, IActionView
    {
        public IActionViewModel ViewModel { get; }

        public CsScriptActionView(CsScriptActionViewModel actionViewModel)
        {
            DataContext = actionViewModel;
            ViewModel = actionViewModel;
            InitializeComponent();
        }
    }
}
