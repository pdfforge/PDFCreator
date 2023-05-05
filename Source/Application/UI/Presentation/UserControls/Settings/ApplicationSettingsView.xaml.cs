using System.Collections.Generic;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    /// <summary>
    ///     Interaction logic for ApplicationSettingsView.xaml
    /// </summary>
    public partial class ApplicationSettingsView : UserControl
    {
        public ApplicationSettingsView(ApplicationSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
