using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public class TokenPlaceHoldersTranslation : ITranslatable
    {
        public string KeywordsFromSettings { get; private set; } = "keyword 1 keyword 2";
        public string InputFilename { get; private set; } = "MyFile";
        public string OutputFilename { get; private set; } = "OutputFilename";
        public string OutputFilename2 { get; private set; } = "OutputFilename2";
        public string OutputFilename3 { get; private set; } = "OutputFilename3";
        public string SubjectFromSettings { get; private set; } = "Subject from Settings";
        public string TitleFromPrintJob { get; private set; } = "Title from Printjob";
        public string TitleFromSettings { get; private set; } = "Title from Settings";

        private string TokenPreviewText { get; set; } = "Value for '{0}'";

        public string FormatTokenPreviewText(string parameterName)
        {
            return string.Format(TokenPreviewText, parameterName);
        }
    }
}
