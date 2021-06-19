using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class PrintFilesTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string ProceedAnyway { get; private set; } = "Do you want to proceed anyway?";
        public string AskSwitchDefaultPrinter { get; private set; } = "PDFCreator needs to temporarily change the default printer to be able to convert the file. Do you want to proceed?";

        private string[] AndXMore { get; set; } = { "and {0} more.", "and {0} more." };
        private string[] NotPrintableFiles { get; set; } = { "The following file can't be converted:", "The following files can't be converted:" };

        public string GetAndXMoreMessage(int numberOfFiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfFiles, AndXMore);
        }

        public string GetNotPrintableFiles(int numberOfFiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfFiles, NotPrintableFiles);
        }
    }
}
