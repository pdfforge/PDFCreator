using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings
{
    public partial class TitleReplacementsView : UserControl
    {
        public TitleReplacementsView(TitleReplacementsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            TransposerHelper.Register(this, vm);
        }

        private void FrameworkElement_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
