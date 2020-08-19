using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class SaveTempOnlyHintControl : UserControl
    {
        public static readonly DependencyProperty SaveTempOnlyCommandDependencyProperty =
            DependencyProperty.Register("SaveTempOnlyCommand", typeof(object), typeof(SaveTempOnlyHintControl),
                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public SaveTempOnlyHintControl()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                DataContext = RestrictedServiceLocator.Current.GetInstance<SaveTempOnlyHintViewModel>();
            }
            InitializeComponent();
        }

        public object SaveTempOnlyCommand
        {
            get { return GetValue(SaveTempOnlyCommandDependencyProperty); }
            set { SetValue(SaveTempOnlyCommandDependencyProperty, value); }
        }
    }
}
