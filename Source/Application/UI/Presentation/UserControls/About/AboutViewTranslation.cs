using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class AboutViewTranslation : ITranslatable
    {
        public string License { get; private set; } = "License";
        public string LicenseInfo { get; private set; } = "PDFCreator is free software consisting of multiple components with individual licenses.";
        public string PleaseReadLicenseSection { get; private set; } = "Please read the license section for further information on these licenses.";
        public string UserGuide { get; private set; } = "User Guide";
        public string KnowledgeBase { get; private set; } = "Knowledge Base";
        public string CommunitySupport { get; private set; } = "Community Support";
        public string Forums { get; private set; } = "(Forums)";
        public string Version { get; private set; } = "Version";
        public string PrioritySupport { get; private set; } = "Priority Support";
    }
}
