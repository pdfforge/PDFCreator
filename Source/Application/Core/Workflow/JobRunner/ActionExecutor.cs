using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IActionExecutor
    {
        void CallPreConversionActions(Job job);

        void CallConversionActions(Job job);

        void CallPostConversionActions(Job job);
    }

    public class ActionExecutor : IActionExecutor
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IActionManager _actionManager;
        private readonly IPdfProcessor _pdfProcessor;
        private readonly IPdfProcessingHelper _processingHelper;

        public ActionExecutor(IActionManager actionManager, IPdfProcessor pdfProcessor, IPdfProcessingHelper processingHelper)
        {
            _actionManager = actionManager;
            _pdfProcessor = pdfProcessor;
            _processingHelper = processingHelper;
        }

        public void CallPreConversionActions(Job job)
        {
            _logger.Trace("Setting up pre conversion actions");
            var preConversionActions = _actionManager.GetActions<IPreConversionAction>(job);

            CallActions(job, preConversionActions);
        }

        public void CallConversionActions(Job job)
        {
            if (_processingHelper.IsProcessingRequired(job))
            {
                _logger.Trace("Setting up conversion actions");
                var conversionActions = _actionManager.GetActions<IConversionAction>(job);

                foreach (var action in conversionActions)
                    action.ProcessJob(_pdfProcessor, job);

                _pdfProcessor.SignEncryptConvertPdfAAndWriteFile(job);
            }
        }

        public void CallPostConversionActions(Job job)
        {
            _logger.Trace("Setting up post conversion actions");
            var postConversionActions = _actionManager.GetActions<IPostConversionAction>(job);

            CallActions(job, postConversionActions);
        }

        private void CallActions(Job job, IEnumerable<IAction> actions)
        {
            _logger.Trace("Starting Actions");
            foreach (var action in actions)
            {
                var result = action.ProcessJob(job);

                if (result)
                    _logger.Trace("Action {0} completed", action.GetType().Name);
                else
                    throw new ProcessingException("An action failed.", result[0]);
            }
        }
    }
}
