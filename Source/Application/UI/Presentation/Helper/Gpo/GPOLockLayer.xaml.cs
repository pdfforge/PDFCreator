using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Gpo
{
    /// <summary>
    /// Interaction logic for GPOLockLayer.xaml
    /// </summary>
    public partial class GPOLockLayer : UserControl
    {
        public static readonly DependencyProperty IsLockedByGpoDependencyProperty = DependencyProperty.Register(
            "IsLockedByGpo",
            typeof(bool),
            typeof(GPOLockLayer), new PropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
        }

        public bool IsLockedByGpo
        {
            get
            {
                var value = GetValue(IsLockedByGpoDependencyProperty);
                return value != null && (bool)value;
            }
            set
            {
                SetValue(IsLockedByGpoDependencyProperty, value);
            }
        }

        public GpoLockLayerViewModel ViewModel { get; }

        public GPOLockLayer()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                ViewModel = RestrictedServiceLocator.Current.GetInstance<GpoLockLayerViewModel>();
            }

            InitializeComponent();
        }
    }
}
