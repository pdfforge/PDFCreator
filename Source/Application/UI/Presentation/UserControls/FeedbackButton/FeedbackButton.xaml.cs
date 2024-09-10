using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    /// <summary>
    /// Interaction logic for FeedbackButton.xaml
    /// </summary>
    public partial class FeedbackButton : UserControl
    {
        public FeedbackButton()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<FeedbackButtonViewModel>();

            InitializeComponent();
        }
    }
}
