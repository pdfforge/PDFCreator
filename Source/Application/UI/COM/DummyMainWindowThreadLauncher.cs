using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class DummyMainWindowThreadLauncher : IMainWindowThreadLauncher
    {
        public void LaunchMainWindow()
        {
        }

        public bool IsPrintJobShellOpen()
        {
            throw new System.NotImplementedException();
        }

        public void SwitchPrintJobShellToMergeWindow()
        {
            throw new System.NotImplementedException();
        }

        public void LaunchMainWindow(StartupRoutine startup)
        {
            throw new System.NotImplementedException();
        }
    }
}
