using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class ApplicationLogoControl : UserControl
    {
        public ApplicationLogoControl()
        {
            ViewCustomization viewCustomization = null;
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                DataContext = RestrictedServiceLocator.Current.GetInstance<ApplicationNameProvider>();
                viewCustomization = RestrictedServiceLocator.Current.GetInstance<ViewCustomization>();
            }

            InitializeComponent();

            CustomEditionText.Text = viewCustomization?.MainWindowText;
            TrialText.Text = viewCustomization?.TrialText;
        }
    }
}
