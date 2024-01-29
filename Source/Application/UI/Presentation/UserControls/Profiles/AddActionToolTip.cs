namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class AddActionToolTip
    {
        public AddActionToolTip(string text, bool isEnabled)
        {
            Text = text;
            IsEnabled = isEnabled;
        }

        public string Text { get; private set; }
        public bool IsEnabled { get; private set; }
    }
}
