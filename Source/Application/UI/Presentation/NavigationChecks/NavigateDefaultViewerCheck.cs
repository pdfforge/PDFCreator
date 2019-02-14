using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.NavigationChecks
{
    public class NavigateDefaultViewerCheck : ISettingsNavigationCheck
    {
        private readonly ICurrentSettings<ObservableCollection<DefaultViewer>> _defaultViewerSettings;
        private readonly IDefaultViewerCheck _defaultViewerCheck;
        private EvaluateSettingsAndNotifyUserTranslation _translation;

        public string ApplicationSettingsRegionName { get; set; } = MainRegionViewNames.SettingsView;

        public NavigateDefaultViewerCheck(ICurrentSettings<ObservableCollection<DefaultViewer>> defaultViewerSettings, IDefaultViewerCheck defaultViewerCheck, ITranslationUpdater translationUpdater)
        {
            _defaultViewerSettings = defaultViewerSettings;
            _defaultViewerCheck = defaultViewerCheck;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
        }

        public SettingsCheckResult CheckSettings()
        {
            var result = new ActionResult();

            foreach (var defaultViewer in _defaultViewerSettings.Settings)
            {
                result.AddRange(_defaultViewerCheck.Check(defaultViewer));
            }

            var resultDict = new ActionResultDict
            {
                { _translation.DefaultViewer, result }
            };

            return new SettingsCheckResult(resultDict, false);
        }

        public bool IsRelevantForRegion(string region)
        {
            return region == ApplicationSettingsRegionName;
        }
    }
}
