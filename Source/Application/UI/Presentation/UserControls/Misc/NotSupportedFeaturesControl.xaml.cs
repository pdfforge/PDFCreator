using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    /// <summary>
    /// Interaction logic for NotSupportedFeaturesControl.xaml
    /// </summary>
    public partial class NotSupportedFeaturesControl : UserControl
    {
        public NotSupportedFeaturesControl()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                DataContext = RestrictedServiceLocator.Current.GetInstance<NotSupportedFeaturesHintViewModel>();
            }
            InitializeComponent();
        }
    }
}
