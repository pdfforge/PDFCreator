using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PasswordButtonControlTranslation : ITranslatable
    {
        //Password Button Control
        public string Ok { get; protected set; } = "_Ok";

        public string Continue { get; protected set; } = "C_ontinue";
        public string Skip { get; protected set; } = "_Skip";
        public string Remove { get; protected set; } = "_Remove";
        public string Cancel { get; protected set; } = "_Cancel";
    }
}
