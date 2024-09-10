using System.Collections.Generic;
using Newtonsoft.Json;
using pdfforge.PDFCreator.UI.Presentation.Windows.Feedback;
using pdfforge.PDFCreator.Utilities;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Data;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Feedback
{
    public class AhaFeedbackSender : IFeedbackSender
    {
        private readonly AhaAppData _ahaAppData;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;

        public AhaFeedbackSender(AhaAppData ahaAppData, ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper)
        {
            _ahaAppData = ahaAppData;
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
        }

        public MultipartFormDataContent GetFormDataContent(string feedbackText, FeedbackType selectedType, CompositeCollection uploadedFiles, string messageTitle)
        {
            var product = _applicationNameProvider.EditionName.ToLowerInvariant().Replace(" ", "_").Trim() switch
            {
                "free" => 33,
                "professional" => 34,
                "server" => 35,
                "terminal_server" => 36,
            };
            var creatorVersion = _versionHelper.FormatWithBuildNumber();
            var application = _applicationNameProvider.ApplicationNameWithEdition;

            var feedbackMessage = new FeedbackMessage
            {
                Message = feedbackText,
                Title = messageTitle,
                Category = (int) selectedType,
                Application = application,
                Version = creatorVersion,
                Product = product
            };

            var content = new MultipartFormDataContent();
            var message = JsonConvert.SerializeObject(feedbackMessage);
            content.Add(new StringContent(message), "message");

            var fileCounter = 1;
            foreach (var uploadedFile in uploadedFiles)
            {
                var filePath = uploadedFile as string;
                var fileName = Path.GetFileName(filePath);
                if (filePath == null)
                    continue;

                var stream = new FileStream(filePath, FileMode.Open);
                var mediaType = GetMediaType(filePath);
                var streamContent = new StreamContent(stream)
                {
                    Headers = { ContentType = MediaTypeHeaderValue.Parse(mediaType) }
                };
                content.Add(streamContent, $"file{fileCounter}", fileName);
                fileCounter++;
            }

            return content;
        }

        private static string GetMediaType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(extension))
                return "application/octet-stream";

            switch (extension.TrimStart('.'))
            {
                // Image media types
                case "bmp":
                    return "image/bmp";
                case "jpg":
                    return "image/jpeg";
                case "gif":
                    return "image/gif";
                case "png":
                    return "image/png";
                case "tif":
                case "tiff":
                    return "image/tiff";
                // Video media type
                case "mp4":
                    return "video/mp4";
                case "mov":
                    return "video/quicktime";
                case "avi":
                    return "video/x-msvideo";
                case "wvm":
                    return "video/x-ms-wmv";
                case "webm":
                    return "video/webm";
                case "flv":
                    return "video/x-flv";
                default:
                    return "application/octet-stream";
            }
        }

        public async Task<HttpResponseMessage> SendFeedbackAsync(MultipartFormDataContent content)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), Urls.AhaUrl);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _ahaAppData.AuthKey);
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            
            request.Content = content;
            return await client.SendAsync(request);
        }
    }
}
