using NLog;
using pdfforge.PDFCreator.Core.StartupInterface;
using System.Collections.Generic;
using pdfforge.PDFCreator.Utilities.Messages;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class CheckAllStartupConditions : ICheckAllStartupConditions
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IEnumerable<IStartupCondition> _startupConditions;
        private readonly IMessageHelper _messageHelper;

        public CheckAllStartupConditions(IList<IStartupCondition> startupConditions, IMessageHelper messageHelper)
        {
            _startupConditions = startupConditions;
            _messageHelper = messageHelper;
        }

        public void CheckAll()
        {
            _logger.Trace("Checking installation...");
            foreach (var startupCondition in _startupConditions)
            {
                _logger.Trace("Checking " + startupCondition.GetType().Name);
                var result = startupCondition.Check();

                if (result.IsSuccessful)
                    continue;

                if (result.ShowMessage)
                {
                    _messageHelper.ShowMessage(result.Message, "PDFCreator", MessageOptions.Ok, MessageIcon.Error);
                }

                if (!string.IsNullOrWhiteSpace(result.Message))
                    _logger.Error(result.Message);

                throw new StartupConditionFailedException(result.ExitCode, result.Message);
            }
        }
    }
}
