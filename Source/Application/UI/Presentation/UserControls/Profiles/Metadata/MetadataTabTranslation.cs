using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class MetadataTabTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Metadata";
        public string TitleLabel { get; private set; } = "Title:";
        public string AuthorLabel { get; private set; } = "Author:";
        public string SubjectLabel { get; private set; } = "Subject:";
        public string KeywordsLabel { get; private set; } = "Keywords:";

        public string NotSupportedMetadata { get; private set; } = "The selected output format does not support metadata";
    }
}
