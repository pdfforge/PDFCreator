using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum ReplacementType
    {
        // The int values reflect the replacement order
        [Translation("RegEx")]
        RegEx = 0,

        [Translation("Start")]
        Start = 1,

        [Translation("End")]
        End = 2,

        // Replace actually does a remove but the enum is not renamed to avoid a settings migration
        [Translation("Remove")]
        Replace = 3,
    }
}