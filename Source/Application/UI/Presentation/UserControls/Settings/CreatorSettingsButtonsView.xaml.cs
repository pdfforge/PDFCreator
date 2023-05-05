using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    public partial class CreatorSettingsButtonsView : UserControl
    {
        public CreatorSettingsButtonsView(CreatorSettingsButtonsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            TransposerHelper.Register(this, vm);
        }

        private void CreatorSettingsButtons_OnLoaded(object sender, RoutedEventArgs e)
        {
            GeneralSettingsButton.Command?.Execute(GeneralSettingsButton.CommandParameter);
        }
    }
}
