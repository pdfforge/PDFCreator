using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Prism.Events;
using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class UpdateIntervalSettingsViewModel : AGeneralSettingsItemControlModel
    {
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IUpdateHelper _updateHelper;
        private readonly ICommandLocator _commandLocator;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<UpdateInterval> _updateIntervalProvider;
        private readonly EditionHelper _editionHelper;
        private readonly IUpdateLauncher _updateLauncher;
        private readonly IOnlineVersionHelper _onlineVersionHelper;
        private readonly SetShowUpdateEvent _showUpdateEvent;

        public UpdateIntervalSettingsViewModel(IUpdateHelper updateHelper, ICommandLocator commandLocator, ApplicationNameProvider applicationNameProvider,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, ITranslationUpdater translationUpdater, 
            IEventAggregator eventAggregator, IInteractionRequest interactionRequest, ICurrentSettings<UpdateInterval> updateIntervalProvider, EditionHelper editionHelper,
            IUpdateLauncher updateLauncher, IOnlineVersionHelper onlineVersionHelper) :
            base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            _applicationNameProvider = applicationNameProvider;
            _updateHelper = updateHelper;
            _commandLocator = commandLocator;
            _interactionRequest = interactionRequest;
            _updateIntervalProvider = updateIntervalProvider;
            _editionHelper = editionHelper;
            _updateLauncher = updateLauncher;
            _onlineVersionHelper = onlineVersionHelper;

            ShouldShowUpdate = updateHelper.UpdateShouldBeShown();
            _showUpdateEvent = eventAggregator.GetEvent<SetShowUpdateEvent>();
            _showUpdateEvent.Subscribe(SetShowDialog);
            currentSettingsProvider.SettingsChanged += (sender, args) =>
            {
                RaisePropertyChanged(nameof(CurrentUpdateInterval));
                RaisePropertyChanged(nameof(DisplayUpdateWarning));
            };
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(TranslatedUpdateInterval));
        }

        private void SetShowDialog(bool value)
        {
            ShouldShowUpdate = _updateHelper.UpdateShouldBeShown();
            RaisePropertyChanged(nameof(ShouldShowUpdate));
            RaisePropertyChanged(nameof(NewUpdateMessage));
        }

        public ICommand InstallUpdateCommand => new AsyncCommand(o => InstallUpdate());
        public ICommand SkipVersionCommand => new DelegateCommand(o => SkipVersion());
        public ICommand AskLaterCommand => new DelegateCommand(o => UpdateLater());

        public ICommand UpdateCheckCommand => new DelegateCommand(async o =>
        {
            try
            {
                if (await _updateHelper.IsUpdateAvailableAsync(false))
                {
                    _showUpdateEvent.Publish(true);
                }
                else
                {
                    var interaction = new MessageInteraction(Translation.NoUpdateMessage, Translation.UpdateCheckTitle, MessageOptions.OK, MessageIcon.PDFCreator);                    
                    _interactionRequest.Raise(interaction);
                }
            }
            catch
            {
                var errorMessage = Translation.GetFormattedErrorMessageWithEditionName(_applicationNameProvider.ApplicationNameWithEdition);
                var interaction = new MessageInteraction(errorMessage, Translation.UpdateCheckTitle, MessageOptions.OK, MessageIcon.Exclamation);
                _interactionRequest.Raise(interaction);
            }
        });

        public string NewUpdateMessage => Translation.GetNewUpdateMessage(_onlineVersionHelper.GetOnlineVersion().Version.ToString(3));

        private void UpdateLater()
        {
            _updateHelper.ShowLater();
            
        }

        private async Task InstallUpdate()
        {
            var applicationVersion = _onlineVersionHelper.GetOnlineVersion();
            await _updateLauncher.LaunchUpdateAsync( applicationVersion);
        }

        private void SkipVersion()
        {
            _updateHelper.SkipVersion();
        }

        public bool UpdateIsEnabled => string.IsNullOrWhiteSpace(GpoSettings?.UpdateInterval);

        public Visibility UpdateCheckControlVisibility
            => _updateHelper.UpdatesEnabled ? Visibility.Visible : Visibility.Collapsed;

        public bool ShouldShowUpdate { get; set; }

        public ICommand VisitWebsiteCommand => _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(PdfforgeWebsiteUrl);
        public string PdfforgeWebsiteUrl => Urls.PdfforgeWebsiteUrl;

        public bool DisplayUpdateWarning => _updateIntervalProvider.Settings == UpdateInterval.Never;

        public UpdateInterval CurrentUpdateInterval
        {
            get
            {
                if (_updateIntervalProvider?.Settings == null)
                    return UpdateInterval.Weekly;

                if (string.IsNullOrWhiteSpace(GpoSettings?.UpdateInterval))
                    return _updateIntervalProvider.Settings;
                return UpdateIntervalHelper.ParseUpdateInterval(GpoSettings.UpdateInterval);
            }
            set
            {
                _updateIntervalProvider.Settings = value;
                RaisePropertyChanged(nameof(CurrentUpdateInterval));
                RaisePropertyChanged(nameof(DisplayUpdateWarning));
            }
        }

        public List<EnumTranslation<UpdateInterval>> TranslatedUpdateInterval
        {
            get
            {
                var translations = Translation.UpdateIntervals.ToList();
                
                if (_editionHelper.IsFreeEdition)
                {
                    var neverUpdateIntervalObject = translations.FirstOrDefault(translation => translation.Value == UpdateInterval.Never);
                    if (neverUpdateIntervalObject != null)
                    {
                        translations.Remove(neverUpdateIntervalObject);
                    }
                }
                return translations;
            } 
        }
    }
}