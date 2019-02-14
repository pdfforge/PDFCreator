using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    /// <summary>
    /// Interaction logic for PreviewPasswordField.xaml
    /// </summary>
    public partial class PreviewPasswordField : UserControl
    {
        #region PreviewPasswordField DependencyProperties

        public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register(
            "PasswordChar",
            typeof(char),
            typeof(PreviewPasswordField), new PropertyMetadata('•'));

        public static readonly DependencyProperty ShownPasswordProperty = DependencyProperty.Register(
            "ShownPassword",
            typeof(string),
            typeof(PreviewPasswordField));

        public static readonly DependencyProperty PasswordTextProperty = DependencyProperty.Register(
            "PasswordText",
            typeof(string),
            typeof(PreviewPasswordField),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        // Static Methods
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PreviewPasswordField view))
                return;

            var newVal = e.NewValue?.ToString();

            if (view.ShownPassword?.Length != newVal?.Length)
            {
                // was set without user input force the value
                view.ShownPassword = view.CreateMaskString(newVal);
            }
        }

        #endregion PreviewPasswordField DependencyProperties

        #region shared Static Method

        internal string CreateMaskString(string toMask)
        {
            if (string.IsNullOrEmpty(toMask))
                return "";

            if (!_isMasked)
                return toMask;

            var builder = new StringBuilder();
            for (var i = 0; i < toMask.Length; i++)
            {
                builder.Append(PasswordChar);
            }

            return builder.ToString();
        }

        #endregion shared Static Method

        #region PreviewPasswordField Code-behind

        // Object variables
        private int _caretIndex;

        internal bool _wasInit;
        private bool _isMasked = true;

        public PreviewPasswordField()
        {
            InitializeComponent();
        }

        public string PasswordText
        {
            get => (string)GetValue(PasswordTextProperty);

            set
            {
                SetValue(PasswordTextProperty, value);
            }
        }

        public char PasswordChar
        {
            get { return (char)GetValue(PasswordCharProperty); }

            set { SetValue(PasswordCharProperty, value); }
        }

        public string ShownPassword
        {
            get => (string)GetValue(ShownPasswordProperty);
            set
            {
                _caretIndex = ClearText.CaretIndex;
                SetValue(ShownPasswordProperty, value);
            }
        }

        private void ClearText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox view) || view.Text == ShownPassword)
            {
                return;
            }

            foreach (var textChange in e.Changes)
            {
                var changedTo = PasswordText;

                if (textChange.RemovedLength > 0)
                {
                    var left = PasswordText.Substring(0, textChange.Offset);

                    var startIndexRight = textChange.Offset + textChange.RemovedLength;
                    var right = PasswordText.Substring(startIndexRight);
                    changedTo = left + right;
                }

                if (textChange.AddedLength > 0)
                {
                    var change = view.Text.Substring(textChange.Offset, textChange.AddedLength);
                    var left = changedTo.Substring(0, textChange.Offset);
                    var right = changedTo.Substring(textChange.Offset, changedTo.Length - textChange.Offset);
                    changedTo = left + change + right;
                }

                ShownPassword = CreateMaskString(changedTo);
                PasswordText = changedTo;
                ClearText.CaretIndex = _caretIndex;
            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMasked = false;
            ShownPassword = CreateMaskString(PasswordText);
            ClearText.CaretIndex = _caretIndex;
            PasswordEyeNormal.Visibility = Visibility.Hidden;
            PasswordEyeClick.Visibility = Visibility.Visible;
        }

        private void UIElement_OnMouseUp(object sender, MouseEventArgs e)
        {
            _isMasked = true;
            ShownPassword = CreateMaskString(PasswordText);
            ClearText.CaretIndex = _caretIndex;

            PasswordEyeNormal.Visibility = Visibility.Visible;
            PasswordEyeClick.Visibility = Visibility.Hidden;
        }

        #endregion PreviewPasswordField Code-behind

        private void Icon_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush brush = (SolidColorBrush)FindResource("NavigationLightGrey");
            EyeGrid.Background = brush;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                OnPreviewMouseDown(this, null);
            }
        }

        private void Icon_OnMouseLeave(object sender, MouseEventArgs e)
        {
            EyeGrid.Background = Brushes.Transparent;
            UIElement_OnMouseUp(this, e);
        }
    }
}
