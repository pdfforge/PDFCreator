using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class ProfilesComboBox : UserControl
    {
        public ObservableCollection<ConversionProfileWrapper> Profiles
        {
            get => (ObservableCollection<ConversionProfileWrapper>)GetValue(ProfilesProperty);
            set => SetValue(ProfilesProperty, value);
        }

        public static readonly DependencyProperty ProfilesProperty =
            DependencyProperty.Register(nameof(Profiles), typeof(ObservableCollection<ConversionProfileWrapper>),
                typeof(ProfilesComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ConversionProfileWrapper SelectedProfile
        {
            get => (ConversionProfileWrapper)GetValue(SelectedProfileProperty);
            set => SetValue(SelectedProfileProperty, value);
        }

        public static readonly DependencyProperty SelectedProfileProperty =
            DependencyProperty.Register(nameof(SelectedProfile), typeof(ConversionProfileWrapper),
                typeof(ProfilesComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedProfileChanged));

        private static void OnSelectedProfileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ((ProfilesComboBox)d).SelectedProfile = e.NewValue as ConversionProfileWrapper;
        }

        public ProfilesComboBox()
        {
            InitializeComponent();
        }
    }
}
