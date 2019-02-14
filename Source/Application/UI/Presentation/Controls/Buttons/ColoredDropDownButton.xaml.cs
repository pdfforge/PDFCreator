using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Buttons
{
    public partial class ColoredDropDownButton : DropDownButton
    {
        public static readonly DependencyProperty PrimaryColorDependencyProperty =
            DependencyProperty.Register("PrimaryColor", typeof(Color), typeof(ColoredDropDownButton), new PropertyMetadata(Colors.Black, OnChangePrimaryColor));

        private static void OnChangePrimaryColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColoredDropDownButton)d).SetColor();
        }

        public static readonly DependencyProperty SecondaryColorDependencyProperty =
            DependencyProperty.Register("SecondaryColor", typeof(Color), typeof(ColoredDropDownButton), new PropertyMetadata(Colors.Red, OnChangeSecondaryColor));

        private static void OnChangeSecondaryColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColoredDropDownButton)d).SetColor();
        }

        private SolidColorBrush _foregroundBrush;
        private SolidColorBrush _backgroundBrush;
        private ColorAnimation _invertedColorAnimation;
        private ColorAnimation _colorAnimation;
        private bool _brushesAreSet;

        public Color PrimaryColor
        {
            get => (Color)GetValue(PrimaryColorDependencyProperty);
            set => SetValue(PrimaryColorDependencyProperty, value);
        }

        public Color SecondaryColor
        {
            get => (Color)GetValue(SecondaryColorDependencyProperty);
            set => SetValue(SecondaryColorDependencyProperty, value);
        }

        public ColoredDropDownButton()
        {
            SetColor();
            InitializeComponent();

            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
            _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseDownAnimationBackground = new ColorAnimation(Color.FromRgb(0xf6, 0xf6, 0xf6), new Duration(TimeSpan.FromSeconds(0)));
            var mouseDownAnimationForeground = new ColorAnimation(PrimaryColor, new Duration(TimeSpan.FromSeconds(0)));
            _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, mouseDownAnimationBackground);
            _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, mouseDownAnimationForeground);
        }

        public void SetColor()
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
                ArrowBrush = _foregroundBrush;
                Foreground = _foregroundBrush;
                ArrowMouseOverBrush = _foregroundBrush;
                ArrowPressedBrush = _foregroundBrush;
                BorderBrush = _foregroundBrush;

                Background = _backgroundBrush;

                _brushesAreSet = true;
            }

            _colorAnimation = new ColorAnimation(PrimaryColor, SecondaryColor, new Duration(TimeSpan.FromSeconds(0.1)));
            _invertedColorAnimation = new ColorAnimation(SecondaryColor, PrimaryColor, new Duration(TimeSpan.FromSeconds(0.1)));
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Opacity = .55;
                return;
            }

            Opacity = 1;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            BorderThickness = new Thickness(0);
            _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _colorAnimation);
            _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _invertedColorAnimation);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            BorderThickness = new Thickness(1);
            _foregroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _invertedColorAnimation);
            _backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, _colorAnimation);
        }
    }
}
