using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Data;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Feedback;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFeedbackSender : IFeedbackSender
    {
        public MultipartFormDataContent GetFormDataContent(string feedbackText, FeedbackType selectedType, CompositeCollection uploadedFiles, string messageTitle)
        {
            return null;
        }

        public Task<HttpResponseMessage> SendFeedbackAsync(MultipartFormDataContent content)
        {
            return null;
        }
    }
}
