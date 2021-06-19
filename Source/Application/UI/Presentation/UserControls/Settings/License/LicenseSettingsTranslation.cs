using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public class LicenseSettingsTranslation : ITranslatable
    {
        public string LastActivation { get; private set; } = "Last activation:";
        public string ActivationFailed { get; private set; } = "Activation failed";
        public string ActivationFailedMessage { get; private set; } = "The activation was not successful:";
        public string ActivationSuccessful { get; private set; } = "Activation successful";
        public string ActivationSuccessfulMessage { get; private set; } = "The activation was successful.";
        public string ActivatedTill { get; private set; } = "Activation valid till:";
        public string EnterLicenseKey { get; private set; } = "Enter license key";
        public string EnterLicenseKeyColon { get; private set; } = "Please enter your license key:";
        public string Licensee { get; private set; } = "Licensee:";
        public string Never { get; private set; } = "Never";
        public string LicenseTabText { get; private set; } = "License";
        public string Expires { get; private set; } = "Expires:";
        public string License { get; private set; } = "License";
        public string LicenseKeyContainsIllegalCharacters { get; private set; } = "The license key contains illegal characters. Valid characters are: A-Z, 0-9 and the dash.";
        public string LicenseKey { get; private set; } = "License Key:";
        public string LicenseStatusActivationExpired { get; private set; } = "The activation has expired. Please renew your activation.";
        public string LicenseStatusBlocked { get; private set; } = "The machine or user is blocked. Please contact the support.";
        public string LicenseStatusError { get; private set; } = "Error while authenticating license.";
        public string LicenseStatusInvalidLicenseKey { get; private set; } = "The license key is not valid. Please enter a new license key.";
        public string LicenseStatusLicenseExpired { get; private set; } = "The license has expired. Please renew your license.";
        public string LicenseStatusNoLicenseKey { get; private set; } = "No license key available. Please enter a new license key.";
        public string LicenseStatusNoServerConnection { get; private set; } = "There was a problem with the connection to the license server.";
        public string LicenseStatusNumberOfActivationsExceeded { get; private set; } = "The license has exceeded the allowed number of activated machines.";
        public string LicenseStatus { get; private set; } = "License status:";
        public string LicenseStatusValid { get; private set; } = "The license is valid.";
        public string LicenseStatusValidForVersionButLicenseExpired { get; private set; } = "The license has expired but is valid for this version.";
        public string LicenseStatusVersionNotCoveredByLicense { get; private set; } = "The license is not valid for this version.";
        public string MachineId { get; private set; } = "Machine ID:";
        public string ManageLicenses { get; private set; } = "Manage licenses";
        public string OfflineActivation { get; private set; } = "Offline activation";
        public string OnlineActivation { get; private set; } = "Online activation";
        public string ValidatingLicense { get; private set; } = "Validating license...";
        private string LicenseKeyHasWrongFormat { get; set; } = "The license key consists of 30 characters or numbers, i.e. {0}";

        public string GetLicenseKeyHasWrongFormatMessage(string formatExample)
        {
            return string.Format(LicenseKeyHasWrongFormat, formatExample);
        }
    }
}
