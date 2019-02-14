using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class PlusFeatureControl : UserControl
    {
        public PlusFeatureControl()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<BusinessFeatureControlViewModel>();

            InitializeComponent();
        }

        public static readonly DependencyProperty EditionProperty = DependencyProperty.Register(
            "Edition", typeof(RequiredEdition), typeof(PlusFeatureControl), new PropertyMetadata(default(RequiredEdition)));

        public RequiredEdition Edition
        {
            get { return (RequiredEdition)GetValue(EditionProperty); }
            set { SetValue(EditionProperty, value); }
        }
    }
}
