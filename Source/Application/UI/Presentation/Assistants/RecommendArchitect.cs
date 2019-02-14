using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class RecommendArchitect : IRecommendArchitect
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IThreadManager _threadManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private bool _isRunning;

        public RecommendArchitect(IInteractionInvoker interactionInvoker, IThreadManager threadManager)
        {
            _interactionInvoker = interactionInvoker;
            _threadManager = threadManager;
        }

        public void Show()
        {
            if (_isRunning)
                return;

            _logger.Info("Recommend PDF Architect");
            var thread = _threadManager.StartSynchronizedUiThread(StartArchitectThread, "RecommendArchitect");
            thread.OnThreadFinished += (sender, args) => _isRunning = false;
        }

        private void StartArchitectThread()
        {
            _isRunning = true;
            _interactionInvoker.Invoke(new RecommendPdfArchitectInteraction(true, false));
        }
    }
}
