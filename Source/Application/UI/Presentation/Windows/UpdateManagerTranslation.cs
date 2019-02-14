using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class UpdateManagerTranslation : ITranslatable
    {
        private string ApplicationUpdate { get; set; } = "{0} Update";
        public string AskLater { get; private set; } = "Ask Later";
        public string DownloadErrorMessage { get; private set; } = "There was a problem during the download. Do you want to download the update from the website instead?";
        public string DownloadHashErrorMessage { get; private set; } = "The MD5 hash of the downloaded file does not match the expected hash. It is possible that the file has been damaged during the download. Do you want to retry the download?";
        private string NewUpdateMessage { get; set; } = "The new version {0} is available. Your current version is {1} from {2}.\nWould you like to download the new version?";

        public string SkipVersion { get; private set; } = "Skip version";

        public string Install { get; private set; } = "Install";
        public string WhatsNew { get; private set; } = "What's new?";
        public string Features { get; private set; } = "Features";
        public string Fixes { get; private set; } = "Fixes";
        public string MiscChanges { get; private set; } = "Other changes";

        public string NewUpdateIsAvailable { get; private set; } = "A new Update is available";

        public string GetNewUpdateMessage(string availableVersion, string currentVersion, string currentVersionDate)
        {
            return string.Format(NewUpdateMessage, availableVersion, currentVersion, currentVersionDate);
        }

        public string GetFormattedTitle(string applicationName)
        {
            return string.Format(ApplicationUpdate, applicationName);
        }
    }
}
