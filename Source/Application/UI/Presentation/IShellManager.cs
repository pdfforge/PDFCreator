using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public interface IShellManager
    {
        void ShowMainShell();

        void ShowPrintJobShell(Job job);

        void MainShellToFront();
    }
}
