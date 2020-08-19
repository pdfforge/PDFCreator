using NLog;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class MainWindowStart : MaybePipedStart
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMainWindowThreadLauncher _mainWindowThreadLauncher;
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IThreadManager _threadManager;

        public MainWindowStart(IThreadManager threadManager, IMaybePipedApplicationStarter maybePipedApplicationStarter, IPdfArchitectCheck pdfArchitectCheck, IMainWindowThreadLauncher mainWindowThreadLauncher)
            : base(maybePipedApplicationStarter)
        {
            _threadManager = threadManager;
            _pdfArchitectCheck = pdfArchitectCheck;
            _mainWindowThreadLauncher = mainWindowThreadLauncher;
        }

        protected override string ComposePipeMessage()
        {
            return "ShowMain|";
        }

        protected override bool StartApplication()
        {
            _logger.Debug("Starting main window");
            _mainWindowThreadLauncher.LaunchMainWindow();

            var pdfArchitectCheckThread = new SynchronizedThread(() => { _pdfArchitectCheck.IsInstalled(); });
            _threadManager.StartSynchronizedThread(pdfArchitectCheckThread);

            return true;
        }
    }
}
