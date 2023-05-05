using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Behaviors
{
    public class AllowableCharactersTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(AllowableCharactersTextBoxBehavior),
                new FrameworkPropertyMetadata(int.MinValue));

        public int MaxValue
        {
            get { return (int)base.GetValue(MaxValueProperty); }
            set { base.SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(AllowableCharactersTextBoxBehavior),
                new FrameworkPropertyMetadata(int.MinValue));

        public int MinValue
        {
            get { return (int)base.GetValue(MinValueProperty); }
            set { base.SetValue(MinValueProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text));

                if (!IsValid(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
                e.Handled = !IsValid(textBox.Text + e.Text);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        }

        private bool IsValid(string newText)
        {
            if (!int.TryParse(newText, out var integerValue))
                return false;

            if (integerValue < MinValue)
                return false;
            if (integerValue > MaxValue)
                return false;

            return true;
        }
    }
}
