using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.UI.Presentation.NavigationChecks
{
    public class SettingsCheckResult
    {
        public SettingsCheckResult()
        {
            Result = new ActionResultDict();
            SettingsHaveChanged = false;
        }

        public SettingsCheckResult(ActionResultDict result, bool settingsHaveChanged)
        {
            Result = result;
            SettingsHaveChanged = settingsHaveChanged;
        }

        public ActionResultDict Result { get; private set; }
        public bool SettingsHaveChanged { get; private set; }

        public void Merge(SettingsCheckResult settingsCheckResult)
        {
            SettingsHaveChanged = SettingsHaveChanged || settingsCheckResult.SettingsHaveChanged;
            Result.Merge(settingsCheckResult.Result);
        }
    }
}
