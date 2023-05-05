using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.CheckBoxes
{
    /// <summary>
    /// Interaction logic for ToggleSwitchButton.xaml
    /// </summary>
    public partial class OnOffLabelToggleSwitchCheckBox : UserControl
    {
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool),
                typeof(OnOffLabelToggleSwitchCheckBox),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        public string OnContent
        {
            get => (string)GetValue(OnContentProperty);
            set => SetValue(OnContentProperty, value);
        }

        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register(nameof(OnContent), typeof(string),
                typeof(OnOffLabelToggleSwitchCheckBox));

        public string OffContent
        {
            get => (string)GetValue(OffContentProperty);
            set => SetValue(OffContentProperty, value);
        }

        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register(nameof(OffContent), typeof(string),
                typeof(OnOffLabelToggleSwitchCheckBox));

        public OnOffLabelToggleSwitchCheckBox()
        {
            InitializeComponent();
        }
    }
}
