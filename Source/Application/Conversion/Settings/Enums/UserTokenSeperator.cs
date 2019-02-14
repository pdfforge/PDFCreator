using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum UserTokenSeperator
    {
        [Translation("[[[   ]]]")]
        SquareBrackets,
        [Translation("<<<    >>>")]
        AngleBrackets,
        [Translation("{{{   }}}")]
        CurlyBrackets,
        [Translation("(((   )))")]
        RoundBrackets
    }
}
