using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands.IniCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class ExportSettingsViewModel : ADebugSettingsItemControlModel
    {

        public ExportSettingsViewModel(
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            IGpoSettings gpoSettings) : base(translationUpdater, gpoSettings)
        {
            LoadIniSettingsCommand = commandLocator.GetCommand<LoadIniSettingsCommand>();
            SaveIniSettingsCommand = commandLocator.GetCommand<SaveSettingsToIniCommand>();
        }

        public ICommand LoadIniSettingsCommand { get; }
        public ICommand SaveIniSettingsCommand { get; }

        public bool ProfileManagementIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableProfileManagement;
            }
        }
    }
}