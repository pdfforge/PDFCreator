using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    /// <summary>
    ///     Interaction logic for LanguageSelectionSettingsView.xaml
    /// </summary>
    public partial class LanguageSelectionSettingsView : UserControl
    {
        public LanguageSelectionSettingsView()
        {
            InitializeComponent();
        }

        public void SetDataContext(AGeneralSettingsItemControlModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
