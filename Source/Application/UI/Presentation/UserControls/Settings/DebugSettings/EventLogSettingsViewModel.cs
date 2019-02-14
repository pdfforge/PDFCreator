using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class EventLogSettingsViewModel : ADebugSettingsItemControlModel
    {
        public ICurrentSettings<ApplicationSettings> ApplicationSettings { get; }
        public string EventLogName { get; set; } = "PDFCreator Server";

        public EventLogSettingsViewModel(ITranslationUpdater translationUpdater, IGpoSettings gpoSettings, ICurrentSettings<ApplicationSettings> applicationSettings)
            : base(translationUpdater, gpoSettings)
        {
            ApplicationSettings = applicationSettings;
            ShowLogFileCommand = new DelegateCommand(ShowLogFileExecute);
        }
        public IEnumerable<LoggingLevel> LoggingValues => Enum.GetValues(typeof(LoggingLevel)) as LoggingLevel[];

        public ICommand ShowLogFileCommand { get; }

        private void ShowLogFileExecute(object o)
        {
            var cmd = $"/c:\"{EventLogName}\"";
            Process.Start("eventvwr.msc", cmd);
        }
    }
}