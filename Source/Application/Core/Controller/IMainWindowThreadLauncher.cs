namespace pdfforge.PDFCreator.Core.Controller
{
    public interface IMainWindowThreadLauncher
    {
        /// <summary>
        ///     Add the Main Window thread to the thread list and start it
        /// </summary>
        void LaunchMainWindow();

        bool IsPrintJobShellOpen();

        void SwitchPrintJobShellToMergeWindow();
    }
}
