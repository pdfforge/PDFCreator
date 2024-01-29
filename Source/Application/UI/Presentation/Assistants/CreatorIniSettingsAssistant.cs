using System.Threading.Tasks;
using pdfforge.DataStorage;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class CreatorIniSettingsAssistant : IniSettingsAssistantBase
    {
        private readonly IIniSettingsLoader _iniSettingsLoader;
        private readonly IActionOrderHelper _actionOrderHelper;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettingsProvider _settingsProvider;

        public CreatorIniSettingsAssistant
            (
            IInteractionInvoker interactionInvoker,
            IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater,
            ISettingsManager settingsManager,
            IDataStorageFactory dataStorageFactory,
            IIniSettingsLoader iniSettingsLoader,
            IPrinterProvider printerProvider,
            IUacAssistant uacAssistant,
            IActionOrderHelper actionOrderHelper)
            : base(printerProvider, uacAssistant, interactionInvoker, interactionRequest, dataStorageFactory, translationUpdater)
        {
            _settingsManager = settingsManager;
            _settingsProvider = settingsManager.GetSettingsProvider();
            _iniSettingsLoader = iniSettingsLoader;
            _actionOrderHelper = actionOrderHelper;
        }

        protected override string ProductName => "PDFCreator";

        protected override async Task<bool> DoLoadIniSettings(string fileName)
        {
            if (_iniSettingsLoader.LoadIniSettings(fileName) is PdfCreatorSettings settings)
            {
                if (!_settingsProvider.CheckValidSettings(settings))
                {
                    await DisplayInvalidSettingsWarning();
                    return false;
                }

                await SyncPrinterMappingWithInstalledPrintersQuery(settings.ApplicationSettings.PrinterMappings);

                _actionOrderHelper.CleanUpAndEnsureValidOrder(settings.ConversionProfiles);

                foreach (var profile in settings.ConversionProfiles)
                {
                    profile.Properties.IsShared = false;
                }

                _settingsManager.ApplyAndSaveSettings(settings);
            }

            return true;
        }

        protected override ISettings GetSettingsCopy()
        {
            return _settingsProvider.Settings.Copy();
        }
    }
}
