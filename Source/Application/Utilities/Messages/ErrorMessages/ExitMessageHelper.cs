using System;
using System.Collections.Generic;
using NLog;

namespace pdfforge.PDFCreator.Utilities.Messages.ErrorMessages
{
    public interface IExitMessageHelper
    {
        void ShowMessage(int errorCode);
    }

    public class ExitMessageHelper : IExitMessageHelper
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, IExitMessageHandler> _errorMessageHandler = new();
        public ExitMessageHelper(IEnumerable<IExitMessageHandler> messageHandlers)
        {
            if(messageHandlers == null)
                throw new ArgumentNullException(nameof(messageHandlers));

            foreach (var errorMessageHandler in messageHandlers)
                _errorMessageHandler.Add(errorMessageHandler.GetExitCode(), errorMessageHandler);
        }

        public void ShowMessage(int errorCode)
        {
            if (_errorMessageHandler.TryGetValue(errorCode, out var value))
            {
                value.HandleExitMessage();
            }
            else
            {
                _logger.Error($@"Error: {errorCode}");
            }
        }
    }
}
