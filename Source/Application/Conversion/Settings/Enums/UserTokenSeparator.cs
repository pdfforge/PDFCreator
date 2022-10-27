using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum UserTokenSeparator
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
