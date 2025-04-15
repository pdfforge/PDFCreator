namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript
{
    public class CsScriptTranslation : ActionTranslationBase
    {
        public string CsScriptDisplayName { get; private set; } = "CS-Script";
        public string CheckScript { get; private set; } = "Check script";
        public string CsScriptFileLabel { get; private set; } = "CS-Script file:";
        public string CsScriptsFolderDoesNotExist { get; private set; } = "The folder 'CS-Scripts' in the application directory does not exist.";
        public string LoadingCsScript { get; private set; } = "Loading CS-Script ...";
        public string LoadingCsScriptSuccessful { get; private set; } = "Loading CS-Script was successful.";
        public string OpenCsScriptsFolder { get; private set; } = "Open CS-Scripts folder";
        public string EnableDebuggingText { get; private set; } = "Enable debugging.";
        public string ReloadScriptList { get; private set; } = "Reload script list";
        private string EnsureCsScriptsFolder { get; set; } = "Please ensure your CS-Scripts are located in:\n\"{0}\"";

        public string GetFormattedLicenseEnsureCsScriptsFolder(string csscriptsFolder)
        {
            return string.Format(EnsureCsScriptsFolder, csscriptsFolder);
        }

        public override string Title { get; set; } = "CS-Script";
        public override string InfoText { get; set; } = "Implement a custom script in C# to process the print job.";
    }
}
