using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string AuthorLabel { get; private set; } = "Author:";

        [Context("PrintJobWindowButton")]
        public string Cancel { get; private set; } = "Cancel";

        [Context("PrintJobWindowButton")]
        public string CancelAll { get; private set; } = "Cancel all";

        public string EditProfile { get; private set; } = "Edit";

        public string KeywordsLabel { get; private set; } = "Keywords:";

        [Context("PrintJobWindowButton")]
        public string Merge { get; private set; } = "Merge";

        [Context("PrintJobWindowButton")]
        public string MergeAll { get; private set; } = "Merge all";

        public string ProfileLabel { get; private set; } = "Profile:";

        [Context("PrintJobWindowButton")]
        public string Save { get; private set; } = "Save";

        public string SaveAs { get; private set; } = "Save as ...";
        public string SaveToDesktop { get; private set; } = "Save to Desktop";

        [Context("PrintJobWindowButton")]
        public string Email { get; private set; } = "E-mail";

        public string SendEmailWithoutSaving { get; private set; } = "Send e-mail without saving";
        public string SubjectLabel { get; private set; } = "Subject:";
        public string TitleLabel { get; private set; } = "Title:";
        public string FilenameText { get; private set; } = "Filename:";
        public string DirectoryLabel { get; private set; } = "Directory:";
        public string SaveTempOnlyIsEnabled { get; private set; } = "You have selected to save files only temporarily. If you want to set the target directory click here.";
        public string RemoveAds { get; private set; } = "Remove ads";
        public string RestrictedActionWarning { get; private set; } = "Certain features are not supported by the selected output format and will be skipped automatically. Please check your profile settings.";
        public string TrialExtendingLicenseInfo { get; private set; } = "Click here to extend your license.";

        protected string[] TrialExpiringInfo { get; private set; } = { "Your trial license will expire in {0} day.", "Your trial license will expire in {0} days." };

        public string GetTrialRemainingDaysInfoText(int trialRemainingDays)
        {
            return PluralBuilder.GetFormattedPlural(trialRemainingDays, TrialExpiringInfo);
        }
    }
}
