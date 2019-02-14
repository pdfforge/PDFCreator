namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class UpdateChange
    {
        public string Text { get; }

        public int Priority { get; }

        public string BulletPoint => "- " + Text;
    }
}
