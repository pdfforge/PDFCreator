using pdfforge.PDFCreator.Core.Services.Translation;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeErrorCodeInterpreter : ErrorCodeInterpreter
    {
        public DesignTimeErrorCodeInterpreter() : base(new TranslationFactory())
        { }
    }
}
