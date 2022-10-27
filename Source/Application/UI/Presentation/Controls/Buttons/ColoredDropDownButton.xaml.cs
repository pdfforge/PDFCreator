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
        private Color _grayedForegroundColor;
        private Color _grayedBackgroundColor;
        private ColorAnimation _toForegroundAnimation;
        private ColorAnimation _toBackgroundAnimation;
        private ColorAnimation _toGrayForegroundAnimation;
        private ColorAnimation _toGrayBackgroundAnimation;
        private SolidColorBrush _backgroundBrush;
        private SolidColorBrush _foregroundBrush;

        public ColoredDropDownButton()
        {
            InitializeComponent();

            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        private Color GetGrayedColor(Color color)
        {
            var grayingFactor = 0.8;
            return Color.FromRgb(
                (byte)(color.R * grayingFactor),
                (byte)(color.G * grayingFactor),
                (byte)(color.B * grayingFactor));
        }

        public void SetColor()
        {
            var foreground = (Color)(Foreground.GetValue(SolidColorBrush.ColorProperty));
            _foregroundBrush = new SolidColorBrush(foreground);
            ArrowBrush = _foregroundBrush;
            ArrowMouseOverBrush = _foregroundBrush;
            ArrowPressedBrush = _foregroundBrush;
            Foreground = _foregroundBrush;
            BorderThickness = new Thickness(0);

            _grayedForegroundColor = GetGrayedColor(foreground);
            _toForegroundAnimation = new ColorAnimation(foreground, new Duration(TimeSpan.FromSeconds(0.1)));
            _toGrayForegroundAnimation = new ColorAnimation(_grayedForegroundColor, new Duration(TimeSpan.FromSeconds(0.1)));

            var backgroundColor = (Color)(Background.GetValue(SolidColorBrush.ColorProperty));
            _backgroundBrush = new SolidColorBrush(backgroundColor);
            Background = _backgroundBrush;

            _grayedBackgroundColor = GetGrayedColor(backgroundColor);
            _toBackgroundAnimation = new ColorAnimation(backgroundColor, new Duration(TimeSpan.FromSeconds(0.1)));
            _toGrayBackgroundAnimation = new ColorAnimation(_grayedBackgroundColor, new Duration(TimeSpan.FromSeconds(0.1)));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Background.BeginAnimation(SolidColorBrush.ColorProperty, _toBackgroundAnimation);
            Foreground.BeginAnimation(SolidColorBrush.ColorProperty, _toForegroundAnimation);
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
            Background.BeginAnimation(SolidColorBrush.ColorProperty, _toBackgroundAnimation);
            Foreground.BeginAnimation(SolidColorBrush.ColorProperty, _toForegroundAnimation);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Background.BeginAnimation(SolidColorBrush.ColorProperty, _toGrayBackgroundAnimation);
            Foreground.BeginAnimation(SolidColorBrush.ColorProperty, _toGrayForegroundAnimation);
        }

        private void Border_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetColor();
        }
    }
}
