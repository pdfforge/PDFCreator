using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class RestartApplicationInteractionTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Update PDFCreator";
        public string Message { get; private set; } = "The Application has to be closed for updating. Close application now?";
        public string Now { get; private set; } = "Close Now";
        public string Later { get; private set; } = "Close manually";
        public string Cancel { get; private set; } = "Cancel";
    }
}
