using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ActionManager : IActionManager
    {
        private readonly IEnumerable<IPreConversionAction> _preConversionActions;
        private readonly IEnumerable<IPostConversionAction> _postConversionActions;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ActionManager(IList<IPreConversionAction> availablePreConversionActions, IList<IPostConversionAction> availablePostConversionActions)
        {
            _preConversionActions = availablePreConversionActions;
            _postConversionActions = availablePostConversionActions;
        }

        public IEnumerable<IPreConversionAction> GetApplicablePreConversionActions(Job job)
        {
            var enabledActions = new List<IPreConversionAction>();

            foreach (var action in _preConversionActions)
            {
                if (!action.IsEnabled(job.Profile))
                    continue;

                enabledActions.Add(action);
                _logger.Trace($"added pre conversion action '{action.GetType().FullName}'");
            }

            return enabledActions;
        }

        public IEnumerable<IPostConversionAction> GetApplicablePostConversionActions(Job job)
        {
            var enabledActions = new List<IPostConversionAction>();

            foreach (var action in _postConversionActions)
            {
                if (!action.IsEnabled(job.Profile))
                    continue;

                enabledActions.Add(action);
                _logger.Trace($"added post conversion action '{action.GetType().FullName}'");
            }

            return enabledActions;
        }
    }
}
