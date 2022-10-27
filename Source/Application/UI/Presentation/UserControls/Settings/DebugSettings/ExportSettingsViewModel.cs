using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands.IniCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

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
            LoadSpecificProfilesCommand = commandLocator.GetCommand<LoadSpecificProfilesCommand>();
            SaveIniSettingsCommand = commandLocator.GetCommand<SaveSettingsToIniCommand>();
        }

        public ICommand LoadIniSettingsCommand { get; }
        public ICommand LoadSpecificProfilesCommand { get; }
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