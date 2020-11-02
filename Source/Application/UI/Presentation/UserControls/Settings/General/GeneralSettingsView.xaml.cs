using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView(GeneralSettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            TransposerHelper.Register(this, vm);
        }
    }
}
