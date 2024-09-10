using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class AboutViewTranslation : ITranslatable
    {
        public string License { get; private set; } = "License Agreement";
        public string UserGuide { get; private set; } = "User Guide";
        public string KnowledgeBase { get; private set; } = "Knowledge Base";
        public string CommunitySupport { get; private set; } = "Community Support";
        public string Forums { get; private set; } = "(Forums)";
        public string PrioritySupport { get; private set; } = "Priority Support";
        public string Feedback { get; private set; } = "Feedback";
    }
}
