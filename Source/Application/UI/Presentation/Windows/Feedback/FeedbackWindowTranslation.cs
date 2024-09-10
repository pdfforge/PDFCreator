using System;
using System.Net;
using System.Security.RightsManagement;
using Microsoft.Identity.Client;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows.Feedback
{
    public class FeedbackWindowTranslation : ITranslatable
    {
        public string ShareYourFeedback { get; private set; } = "Share your feedback";
        public string DescribeYourFeedback { get; private set; } = "Describe your feedback";
        public string EmailLabel { get; private set; } = "Email";
        public string EmailPlaceholder { get; private set; } = "Email address";
        public string FeedbackPlaceholder { get; private set; } = "Please provide details here";
        public string FeedbackType { get; private set; } = "What type of feedback would you like to send?";
        public string PleaseSelectFeedbackType { get; private set; } = "Please select a feedback type";
        public string ReportIssue { get; private set; } = "I need to report an issue";
        public string PositiveFeedback { get; private set; } = "I want to share positive feedback";
        public string FeatureSuggestion { get; private set; } = "I would like to suggest a feature";
        public string Send { get; private set; } = "Send";
        public string SendUsFeedback { get; private set; } = "Send us your feedback";
        public string AttachFile { get; private set; } = "Attach a file";
        public string ImageFiles { get; private set; } = "Image files";
        public string PdfFiles { get; private set; } = "PDF files";
        public string VideoFiles { get; private set; } = "Video files";
        public string AllFiles { get; private set; } = "All files";
        public string File { get; private set; } = "File:";

        public string ErrorMessage { get; private set; } = "An error occured while attempting to submit your feedback";
        public string Error { get; private set; } = "Error";

    }
}
