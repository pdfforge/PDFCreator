using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Globalization;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class OutputFormatPdfView : UserControl
    {
        public OutputFormatPdfView(OutputFormatPdfViewModel vm)
        {
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }

        // This can stay in the code behind, can't easily be done in viewmodel because the textbox is bound to a double
        private void JpegFactorTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var cursorPosition = JpegFactorTextBox.SelectionStart;
            var numberFormat = CultureInfo.CurrentCulture.NumberFormat;

            // To help the user, we we replace thousand separators with decimal separators if both are set (i.e. ',' to '.' or vice versa)
            if (!string.IsNullOrEmpty(numberFormat.NumberDecimalSeparator) && !string.IsNullOrEmpty(numberFormat.NumberGroupSeparator))
            {
                JpegFactorTextBox.Text = JpegFactorTextBox.Text.Replace(numberFormat.NumberGroupSeparator, numberFormat.NumberDecimalSeparator);
            }

            JpegFactorTextBox.SelectionStart = cursorPosition;
        }
    }
}
