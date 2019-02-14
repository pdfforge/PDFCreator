using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Buttons
{
    /// <summary>
    /// Interaction logic for CustomColoredButton.xaml
    /// </summary>
    public partial class CustomColoredButton : Button
    {
        public static readonly DependencyProperty PrimaryColorDependencyProperty =
            DependencyProperty.Register("PrimaryColor", typeof(Color), typeof(CustomColoredButton), new PropertyMetadata(Colors.Aqua, OnChangePrimaryColor));

        private static void OnChangePrimaryColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CustomColoredButton)d).SetColor();
        }

        public Color PrimaryColor
        {
            get => (Color)GetValue(PrimaryColorDependencyProperty);
            set => SetValue(PrimaryColorDependencyProperty, value);
        }

        public static readonly DependencyProperty SecondaryColorDependencyProperty =
            DependencyProperty.Register("SecondaryColor", typeof(Color), typeof(CustomColoredButton), new PropertyMetadata(Colors.Magenta, OnChangeSecondaryColor));

        private static void OnChangeSecondaryColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CustomColoredButton)d).SetColor();
        }

        public Color SecondaryColor
        {
            get => (Color)GetValue(SecondaryColorDependencyProperty);
            set => SetValue(SecondaryColorDependencyProperty, value);
        }

        private SolidColorBrush _foregroundBrush;
        private SolidColorBrush _backgroundBrush;

        private ColorAnimation _invertedColorAnimation;
        private ColorAnimation _leaveColorAnimation;
        private ColorAnimation _colorAnimation;

        private bool _brushesAreSet;

        public CustomColoredButton()
        {
            InitializeComponent();
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            SetColor();
        }

        public virtual void SetColor()
        {
            if (_backgroundBrush == null)
                _backgroundBrush = new SolidColorBrush(PrimaryColor);
            else
                _backgroundBrush.Color = PrimaryColor;

            if (_foregroundBrush == null)
                _foregroundBrush = new SolidColorBrush(SecondaryColor);
            else
                _foregroundBrush.Color = SecondaryColor;

            if (!_brushesAreSet)
            {
                Foreground = _foregroundBrush;

                Background = _backgroundBrush;

                _brushesAreSet = true;
            }

            BorderBrush = new SolidColorBrush(PrimaryColor);
            _colorAnimation = new ColorAnimation(PrimaryColor, SecondaryColor, new Duration(TimeSpan.FromSeconds(0.15)));
            _invertedColorAnimation = new ColorAnimation(SecondaryColor, PrimaryColor, new Duration(TimeSpan.FromSeconds(0.15)));
            _leaveColorAnimation = new ColorAnimation(SecondaryColor, PrimaryColor, new Duration(TimeSpan.FromSeconds(0.1)));
        }

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);
            if ((bool)e.NewValue)
            {
                var mouseDownAnimationBackground = new ColorAnimation(SecondaryColor, Color.FromRgb(0xf6, 0xf6, 0xf6), new Duration(TimeSpan.FromSeconds(0)));
                var mouseDownAnimationForeground = new ColorAnimation(PrimaryColor, new Duration(TimeSpan.FromSeconds(0)));
                _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, mouseDownAnimationBackground);
                _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, mouseDownAnimationForeground);
            }
            else
            {
                _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(SecondaryColor, new Duration(TimeSpan.FromSeconds(0))));
                _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(PrimaryColor, new Duration(TimeSpan.FromSeconds(0))));
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            BorderThickness = new Thickness(0);
            _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(SecondaryColor, new Duration(TimeSpan.FromSeconds(0))));
            _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _leaveColorAnimation);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            BorderThickness = new Thickness(1);
            _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _invertedColorAnimation);
            _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _colorAnimation);
        }
    }
}
