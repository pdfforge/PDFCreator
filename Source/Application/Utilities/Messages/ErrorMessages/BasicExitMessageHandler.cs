using NLog;

namespace pdfforge.PDFCreator.Utilities.Messages.ErrorMessages;

public class BasicExitMessageHandler(int errorCode) : IExitMessageHandler
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public void HandleExitMessage()
    {
        // simple implementation without view, extendable to add view
        _logger.Error($@"Error: {errorCode}");
    }

    public int GetExitCode()
    {
        return errorCode;
    }
}
