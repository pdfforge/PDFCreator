using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellLauncher : IMainWindowThreadLauncher
    {
        private readonly IShellManager _shellManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IThreadManager _threadManager;
        private ISynchronizedThread _mainWindowThread;

        public MainShellLauncher(IThreadManager threadManager, IShellManager shellManager, IEventAggregator eventAggregator)
        {
            _threadManager = threadManager;
            _shellManager = shellManager;
            _eventAggregator = eventAggregator;
        }

        public void LaunchMainWindow()
        {
            if (_mainWindowThread != null)
            {
                _shellManager.MainShellToFront();
                return;
            }
            _mainWindowThread = _threadManager.StartSynchronizedUiThread(MainWindowLaunchThreadMethod, "MainWindowThread");
        }

        private void MainWindowLaunchThreadMethod()
        {
            try
            {
                _shellManager.ShowMainShell();
            }
            finally
            {
                _mainWindowThread = null;
            }
        }

        public void SwitchPrintJobShellToMergeWindow()
        {
            _eventAggregator.GetEvent<ManagePrintJobEvent>().Publish();
        }

        public bool IsPrintJobShellOpen()
        {
            return _shellManager.PrintJobShellIsOpen;
        }
    }
}
