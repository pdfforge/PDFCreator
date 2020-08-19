using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class BusinessFeatureBadge : UserControl
    {
        public BusinessFeatureBadge()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<BusinessFeatureBadgeViewModel>();

            InitializeComponent();
        }

        public static readonly DependencyProperty EditionProperty = DependencyProperty.Register(
            "Edition", typeof(RequiredEdition), typeof(BusinessFeatureBadge), new PropertyMetadata(default(RequiredEdition)));

        public RequiredEdition Edition
        {
            get { return (RequiredEdition)GetValue(EditionProperty); }
            set { SetValue(EditionProperty, value); }
        }
    }
}
