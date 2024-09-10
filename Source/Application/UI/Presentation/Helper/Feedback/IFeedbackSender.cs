using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Data;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Feedback
{
    public interface IFeedbackSender
    {
        MultipartFormDataContent GetFormDataContent(string feedbackText, FeedbackType selectedType, CompositeCollection uploadedFiles, string messageTitle);

        Task<HttpResponseMessage> SendFeedbackAsync(MultipartFormDataContent content);
    }
}
