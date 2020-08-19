using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public double EntropyPercentage
        {
            get { return (double)GetValue(EntropyPercentageProperty); }
            set
            {
                SetValue(EntropyPercentageProperty, value);
            }
        }

        public static readonly DependencyProperty EntropyPercentageProperty =
            DependencyProperty.Register("EntropyPercentage", typeof(double),
                typeof(PreviewPasswordField),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EntropyPercentagePropertyChangedCallback));

        private static void EntropyPercentagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PreviewPasswordField).EntropyPercentage = (double)e.NewValue;

            (d as PreviewPasswordField).EntropyIndicatorWidth = (double)e.NewValue * 5;
        }

        public double EntropyIndicatorWidth
        {
            get { return (double)GetValue(EntropyIndicatorWidthProperty); }
            set { SetValue(EntropyIndicatorWidthProperty, value); }
        }

        public static readonly DependencyProperty EntropyIndicatorWidthProperty =
            DependencyProperty.Register("EntropyIndicatorWidth", typeof(double),
                typeof(PreviewPasswordField), new PropertyMetadata(0.0));

        public bool IsEntropyChecked
        {
            get { return (bool)GetValue(IsEntropyCheckedProperty); }
            set { SetValue(IsEntropyCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsEntropyCheckedProperty =
            DependencyProperty.Register("IsEntropyChecked", typeof(bool), typeof(PreviewPasswordField),
                new PropertyMetadata(false));

        public string PasswordIndicatorLabel
        {
            get { return (string)GetValue(PasswordIndicatorLabelProperty); }
            set { SetValue(PasswordIndicatorLabelProperty, value); }
        }

        public static readonly DependencyProperty PasswordIndicatorLabelProperty =
            DependencyProperty.Register("PasswordIndicatorLabel", typeof(string), typeof(PreviewPasswordField),
                new PropertyMetadata(""));

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
            SolidColorBrush brush = (SolidColorBrush)FindResource("NavigationLightGreyBrush");
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

    public class ProgressForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double progress = Math.Min(Double.Parse(value.ToString()) / 100, 1);

            Brush foreground = new SolidColorBrush(Colors.Transparent);
            if (progress > 0.02 && progress < 0.5)
                foreground = new SolidColorBrush(Color.FromRgb(255, (byte)((progress * 1.5) * 255), 0));
            if (progress > 0.5 && progress < 0.65)
                foreground = new SolidColorBrush(Color.FromRgb((byte)((1 - progress) * 255), (byte)(progress * 255), 0));
            if (progress >= 0.65)
                foreground = new SolidColorBrush(Color.FromRgb(100, 170, 0));

            return foreground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
