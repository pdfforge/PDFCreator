using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.Translations
{
    public class ProgramTranslation : ITranslatable
    {
        private string ErrorWithLicensedComponent { get; set; } = "There was an error with a component licensed by pdfforge.\nPlease reinstall PDFCreator. (Error {0})";
        private string LicenseInvalid { get; set; } = "Your license for \"{0}\" is invalid or has expired. Please check your license, otherwise PDFCreator will shutdown.\nDo you want to check your license now?";
        private string LicenseInvalidAfterReactivation { get; set; } = "Your license for \"{0}\" has expired.\nPDFCreator will shut down.";
        private string LicenseInvalidGpoHideLicenseTab { get; set; } = "Your license for \"{0}\" has expired.\nPlease contact your administrator.\nPDFCreator will shut down.";
        private string TrialExpired { get; set; } = "Your trial has expired on {0}.\nPDFCreator will shut down.\n\nPlease contact: licensing@pdfforge.org";
        private string LicenseKeyNotFound { get; set; } = "A license key for {0} could not be found. Please provide your license key, otherwise PDFCreator will shutdown. \nDo you want to activate your license now?";
        public string LicenseKeyNotFoundAfterReactivation { get; set; } = "A license key could not be found.\nPDFCreator will shut down.";
        public string UsePDFCreatorTerminalServer { get; private set; } = "Please use \"PDFCreator Terminal Server\" for use on computers with installed Terminal Services.\n\nPlease visit our website for more information or contact us directly: licensing@pdfforge.org";
        

        public string GetFormattedLicenseInvalidGpoHideLicenseTab(string editionWithVersionNumber)
        {
            return string.Format(LicenseInvalidGpoHideLicenseTab, editionWithVersionNumber);
        }

        public string GetFormattedLicenseInvalidTranslation(string editionWithVersionNumber)
        {
            return string.Format(LicenseInvalid, editionWithVersionNumber);
        }

        public string GetFormattedLicenseInvalidAfterReactivationTranslation(string applicationName)
        {
            return string.Format(LicenseInvalidAfterReactivation, applicationName);
        }

        public string GetFormattedErrorWithLicensedComponentTranslation(int errorCode)
        {
            return string.Format(ErrorWithLicensedComponent, errorCode);
        }

        public string GetFormattedTrialExpiredTranslation(string trialExpireDate)
        {
            return string.Format(TrialExpired, trialExpireDate);
        }

        public string GetFormattedLicenseKeyNotFoundTranslation(string editionWithVersionNumber)
        {
            return string.Format(LicenseKeyNotFound, editionWithVersionNumber);
        }
    }
}
