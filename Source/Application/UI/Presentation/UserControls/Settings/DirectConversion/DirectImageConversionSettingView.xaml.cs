using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;


namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion
{
    /// <summary>
    /// Interaction logic for DirectImageConversionSettingView.xaml
    /// </summary>
    public partial class DirectImageConversionSettingView : UserControl
    {
        public DirectImageConversionSettingView(DirectImageConversionSettingsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            TransposerHelper.Register(this, vm);
        }
    }
}
