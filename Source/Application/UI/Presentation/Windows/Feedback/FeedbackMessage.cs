using Newtonsoft.Json;

namespace pdfforge.PDFCreator.UI.Presentation.Windows.Feedback
{
    public class FeedbackMessage
    {
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("firstName", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("lastName", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public long? Category { get; set; }

        [JsonProperty("application", NullValueHandling = NullValueHandling.Ignore)]
        public string Application { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("product", NullValueHandling = NullValueHandling.Ignore)]
        public long? Product { get; set; }
    }
}
