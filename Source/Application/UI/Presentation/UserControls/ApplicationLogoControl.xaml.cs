using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Controls;
using pdfforge.PDFCreator.Core.SettingsManagement.Customization;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class ApplicationLogoControl : UserControl
    {
        public ApplicationLogoControl()
        {
            ViewCustomization viewCustomization = null;
            InitializeComponent();
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                DataContext = RestrictedServiceLocator.Current.GetInstance<ApplicationNameProvider>();
                viewCustomization = RestrictedServiceLocator.Current.GetInstance<ViewCustomization>();

                var highlight = RestrictedServiceLocator.Current.GetInstance<HighlightColorRegistration>();
                highlight.RegisterHighlightColorResource(this);
            }

            CustomEditionText.Text = viewCustomization?.MainWindowText;
            TrialText.Text = viewCustomization?.TrialText;
        }


    }
}
