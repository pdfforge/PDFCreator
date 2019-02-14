using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public partial class WelcomeView : UserControl, IInteraction
    {
        public WelcomeView()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<WelcomeViewModel>();

            InitializeComponent();
        }
    }
}
