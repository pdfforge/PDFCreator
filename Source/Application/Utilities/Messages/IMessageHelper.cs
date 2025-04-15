namespace pdfforge.PDFCreator.Utilities.Messages
{
    public interface IMessageHelper
    {
        /// <summary>
        /// Shows a message with given message, title, options, icon
        /// </summary>
        /// <param name="message">The message text of the shown message</param>
        /// <param name="title">A title that can be shown for the message</param>
        /// <param name="options">Options for possible user responses</param>
        /// <param name="icon">Message icon for a visual message</param>
        /// <param name="happyPathResponse">automatic response in case user input is not possible</param>
        /// <returns>User response based of MessageResponse enum</returns>
        MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon, MessageResponse happyPathResponse = MessageResponse.Cancel);
        void ShowHelp(string helpFile,string topic);
    }
}
