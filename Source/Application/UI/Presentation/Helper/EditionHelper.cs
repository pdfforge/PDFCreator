namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class EditionHelper
    {
        public EditionHelper(bool isFreeEdition)
        {
            IsFreeEdition = isFreeEdition;
        }

        public bool IsFreeEdition { get; }
    }
}
