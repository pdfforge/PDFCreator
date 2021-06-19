using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IActionExecutor
    {
        void ApplyRestrictions(Job job);

        bool IsProcessingRequired(Job job);

        void CallPreConversionActions(Job job);

        void CallConversionActions(Job job);

        void CallPostConversionActions(Job job);
    }

    public class ActionExecutor : IActionExecutor
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IActionManager _actionManager;
        private readonly IPdfProcessor _pdfProcessor;

        public ActionExecutor(IActionManager actionManager, IPdfProcessor pdfProcessor)
        {
            _actionManager = actionManager;
            _pdfProcessor = pdfProcessor;
        }

        public void ApplyRestrictions(Job job)
        {
            var actions = _actionManager.GetEnabledActionsInCurrentOrder<IAction>(job);
            foreach (var action in actions)
                action.ApplyRestrictions(job);
        }

        public bool IsProcessingRequired(Job job)
        {
            if (job.Profile.OutputFormat.IsPdfA())
                return true;

            var actions = _actionManager.GetEnabledActionsInCurrentOrder<IConversionAction>(job);
            foreach (var action in actions)
            {
                if (!action.IsRestricted(job.Profile))
                    return true;
            }

            return false;
        }

        public void CallPreConversionActions(Job job)
        {
            _logger.Trace("Setting up pre conversion actions");
            var preConversionActions = _actionManager.GetEnabledActionsInCurrentOrder<IPreConversionAction>(job);

            CallActions(job, preConversionActions);
        }

        public void CallConversionActions(Job job)
        {
            if (IsProcessingRequired(job))
            {
                _logger.Trace("Setting up conversion actions");
                var conversionActions = _actionManager.GetEnabledActionsInCurrentOrder<IConversionAction>(job);

                foreach (var action in conversionActions)
                    if (!action.IsRestricted(job.Profile))
                        action.ProcessJob(_pdfProcessor, job);

                _pdfProcessor.SignEncryptConvertPdfAAndWriteFile(job);
            }
        }

        public void CallPostConversionActions(Job job)
        {
            _logger.Trace("Setting up post conversion actions");
            var postConversionActions = _actionManager.GetEnabledActionsInCurrentOrder<IPostConversionAction>(job);

            var profile = job.Profile;
            CallActions(job, postConversionActions, profile.SkipSendFailures);
        }

        private void CallActions(Job job, IEnumerable<IAction> actions, bool skipFailures = false)
        {
            _logger.Trace("Starting Actions");
            var failureList = new ActionResult();
            foreach (var action in actions)
            {
                if (action.IsRestricted(job.Profile))
                    continue;

                var result = action.ProcessJob(job);
                if (result)
                    _logger.Trace("Action {0} completed", action.GetType().Name);
                else if (skipFailures)
                    failureList.Add(result);
                else
                    throw new ProcessingException("An action failed.", result[0]);
            }

            if (!failureList)
                throw new AggregateProcessingException("One or more actions failed.", failureList);
        }
    }
}
