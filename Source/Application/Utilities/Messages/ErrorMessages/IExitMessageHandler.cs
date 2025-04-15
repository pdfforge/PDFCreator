namespace pdfforge.PDFCreator.Utilities.Messages.ErrorMessages;

public interface IExitMessageHandler
{
    void HandleExitMessage();

    int GetExitCode();
}
