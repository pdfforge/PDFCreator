using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class RecommendArchitectUpgrade : IRecommendArchitectUpgrade
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IThreadManager _threadManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private bool _isRunning;

        public RecommendArchitectUpgrade(IInteractionInvoker interactionInvoker, IThreadManager threadManager)
        {
            _interactionInvoker = interactionInvoker;
            _threadManager = threadManager;
        }

        public void Show()
        {
            if (_isRunning)
                return;

            _logger.Info("Recommend PDF Architect Upgrade");
            var thread = _threadManager.StartSynchronizedUiThread(StartArchitectThread, "RecommendArchitectUpgrade");
            thread.OnThreadFinished += (sender, args) => _isRunning = false;
        }

        private void StartArchitectThread()
        {
            _isRunning = true;
            _interactionInvoker.Invoke(new RecommendPdfArchitectInteraction(true, true));
        }
    }

    public interface IRecommendArchitectUpgrade
    {
        void Show();
    }
}
