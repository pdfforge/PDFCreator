using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands.IniCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class ExportSettingsViewModel : ADebugSettingsItemControlModel
    {
        private readonly EditionHelper _editionHelper;
        private readonly IIniSettingsAssistant _iniSettingsAssistant;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;

        public ExportSettingsViewModel(
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            IGpoSettings gpoSettings,
            EditionHelper editionHelper,
            IIniSettingsAssistant iniSettingsAssistant,
            IInteractionRequest interactionRequest,
            IEventAggregator eventAggregator
        ) : base(translationUpdater, gpoSettings)
        {
            _editionHelper = editionHelper;
            _iniSettingsAssistant = iniSettingsAssistant;
            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;

            LoadIniSettingsCommand = new AsyncCommand<bool>(LoadSettingsExecute);
            LoadSpecificProfilesCommand = new AsyncCommand<bool>(LoadSpecificProfilesExecute);
            SaveIniSettingsCommand = commandLocator.GetCommand<SaveSettingsToIniCommand>();
        }

        public AsyncCommand<bool> LoadIniSettingsCommand { get; }
        public AsyncCommand<bool> LoadSpecificProfilesCommand { get; }
        public ICommand SaveIniSettingsCommand { get; }

        private async Task<bool> LoadSettingsExecute(object o)
        {
            _eventAggregator.GetEvent<SettingsLoadingEvent>().Publish(true);
            await _iniSettingsAssistant.LoadIniSettings();
            _eventAggregator.GetEvent<SettingsLoadingEvent>().Publish(false);
            return true;
        }

        private async Task<bool> LoadSpecificProfilesExecute(object o)
        {
            _eventAggregator.GetEvent<SettingsLoadingEvent>().Publish(true);
            var interaction = new LoadSpecificProfileInteraction();
            await _interactionRequest.RaiseAsync(interaction);
            _eventAggregator.GetEvent<SettingsLoadingEvent>().Publish(false);
            return true;
        }

        public bool ProfileManagementIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableProfileManagement;
            }
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(LoadSpecificProfilesQueues));
        }

        public string LoadSpecificProfilesQueues => _editionHelper.IsServer ? Translation.LoadSpecificQueuesFromFile : Translation.LoadSpecificProfilesFromFile;
    }
}