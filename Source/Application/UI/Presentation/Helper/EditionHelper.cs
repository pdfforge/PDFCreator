namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class EditionHelper
    {
        public EditionHelper(bool showOnlyForPlusAndBusiness, bool showOnlyForBusiness)
        {
            ShowOnlyForPlusAndBusiness = showOnlyForPlusAndBusiness;
            ShowOnlyForBusiness = showOnlyForBusiness;
        }

        public bool ShowOnlyForPlusAndBusiness { get; }
        public bool ShowOnlyForBusiness { get; }
        public bool IsFreeEdition => ShowOnlyForPlusAndBusiness;
    }
}
