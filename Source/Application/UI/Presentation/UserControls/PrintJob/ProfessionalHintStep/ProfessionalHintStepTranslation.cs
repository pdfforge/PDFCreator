using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.ProfessionalHintStep
{
    public class ProfessionalHintStepTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        private string[] ThankYou { get; set; } = { "After converting {0} file, we have a recommendation for you:", "After converting {0} files, we have a recommendation for you:" };

        public string GetThankYouMessage(int numberOfPrintJobs)
        {
            return PluralBuilder.GetFormattedPlural(numberOfPrintJobs, ThankYou);
        }

        public string HigherEncryption { get; private set; } = "Higher encryption (256 Bit AES)";
        public string HotFolder { get; private set; } = "Includes HotFolder";
        public string UserTokens { get; private set; } = "Extract settings with user tokens";
        public string TrayNotifications { get; private set; } = "Automatic saving with tray notification";
        public string PrioritySupport { get; private set; } = "Priority support";
        public string AdminFriendly { get; private set; } = "Admin friendly";

        
        public string NoThanksButtonContent { get; private set; } = "No, thanks";
        public string MoreInformation { get; private set; } = "More Information";
    }
}
