using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{

    public partial class LoadSpecificProfileView : UserControl
    {
        public LoadSpecificProfileView(LoadSpecificProfileViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
