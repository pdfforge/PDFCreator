using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.Behavior
{
    public class TextBoxForceCaretToEndBehavior : Behavior<UIElement>
    {
        private TextBox _textBox;

        protected override void OnAttached()
        {
            base.OnAttached();

            _textBox = AssociatedObject as TextBox;

            if (_textBox == null)
            {
                return;
            }

            _textBox.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            if (_textBox == null)
            {
                return;
            }
            _textBox.GotFocus -= OnTextChanged;

            base.OnDetaching();
        }

        private void OnTextChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            _textBox.CaretIndex = _textBox.Text.Length;
            _textBox.TextChanged -= OnTextChanged;
        }
    }
}
