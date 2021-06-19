namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public interface IStatusHintViewModel
    {
        string StatusText { get; }
        bool HasWarning { get; }
        bool HideStatusInOverlay { get; }
    }
}
