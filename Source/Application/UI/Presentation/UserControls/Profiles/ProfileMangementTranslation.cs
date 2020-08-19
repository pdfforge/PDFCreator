using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class ProfileMangementTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string AddNewProfile { get; private set; } = "Add New Profile";
        public string NewProfile { get; private set; } = "New Profile";
        public string EnterProfileName { get; private set; } = "Please enter profile name:";
        public string EnterNewProfileName { get; private set; } = "Please enter new profile name:";
        public string InvalidProfileName { get; private set; } = "The entered profile name is invalid or does already exist.";
        public string ProfileLabelText { get; private set; } = "Profile:";
        public string RenameButton { get; private set; } = "Rename";
        public string RenameProfile { get; private set; } = "Rename Profile";
        public string AddButton { get; private set; } = "Add";
        public string RemoveButton { get; private set; } = "Remove";
        public string RemoveProfile { get; private set; } = "Remove Profile";
		public string RemoveProfileForSure { get; private set; } = "Are you sure, you want to remove this profile?";

        private string[] ProfileIsMappedTo { get; set; } = { "The profile is mapped to the following printer:", "The profile is mapped to the following printers:" };

        public string GetProfileIsMappedToMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, ProfileIsMappedTo);
        }

        private string[] PrinterWillBeMappedTo { get; set; } = { "This printer will be mapped to the default profile.", "These printers will be mapped to the default profile." };

        public string GetPrinterWillBeMappedToMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, PrinterWillBeMappedTo);
        }
    }
}
