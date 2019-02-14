using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;
using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class UpdateIntervalSettingsViewModel : AGeneralSettingsItemControlModel
    {
        private readonly IProcessStarter _processStarter;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IUpdateAssistant _updateAssistant;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<UpdateInterval> _updateIntervalProvider;
        private readonly EditionHelper _editionHelper;
        private readonly IUpdateLauncher _updateLauncher;
        private readonly SetShowUpdateEvent _showUpdateEvent;

        public UpdateIntervalSettingsViewModel(IUpdateAssistant updateAssistant, IProcessStarter processStarter, ApplicationNameProvider applicationNameProvider,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, ITranslationUpdater translationUpdater, 
            IEventAggregator eventAggregator, IInteractionRequest interactionRequest, ICurrentSettings<UpdateInterval> updateIntervalProvider, EditionHelper editionHelper,
            IUpdateLauncher updateLauncher) :
            base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            _processStarter = processStarter;
            _applicationNameProvider = applicationNameProvider;
            _updateAssistant = updateAssistant;
            _interactionRequest = interactionRequest;
            _updateIntervalProvider = updateIntervalProvider;
            _editionHelper = editionHelper;
            _updateLauncher = updateLauncher;

            ShowUpdate = updateAssistant.ShowUpdate;
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
            ShowUpdate = _updateAssistant.ShowUpdate;
            RaisePropertyChanged(nameof(ShowUpdate));
            RaisePropertyChanged(nameof(NewUpdateMessage));
        }

        public ICommand InstallUpdateCommand => new AsyncCommand(o => InstallUpdate());
        public ICommand SkipVersionCommand => new DelegateCommand(o => SkipVersion());
        public ICommand AskLaterCommand => new DelegateCommand(o => UpdateLater());

        public ICommand UpdateCheckCommand => new DelegateCommand(o =>
        {
            try
            {
                if (_updateAssistant.IsOnlineUpdateAvailable(false))
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

        public string NewUpdateMessage => Translation.GetNewUpdateMessage(_updateAssistant.OnlineVersion.Version.ToString(3));

        private void UpdateLater()
        {
            _updateAssistant.SetNewUpdateTime();
        }

        private async Task InstallUpdate()
        {
            await _updateLauncher.LaunchUpdate(_updateAssistant.OnlineVersion);
        }

        private void SkipVersion()
        {
            _updateAssistant.SkipVersion();
        }

        public bool UpdateIsEnabled => GpoSettings?.UpdateInterval == null;

        public Visibility UpdateCheckControlVisibility
            => _updateAssistant.UpdatesEnabled ? Visibility.Visible : Visibility.Collapsed;

        public bool ShowUpdate { get; set; }

        public ICommand VisitWebsiteCommand => new DelegateCommand(VisitWebsiteExecute);
        public string PdfforgeWebsiteUrl => Urls.PdfforgeWebsiteUrl;

        public bool DisplayUpdateWarning => _updateIntervalProvider.Settings == UpdateInterval.Never;

        public UpdateInterval CurrentUpdateInterval
        {
            get
            {
                if (_updateIntervalProvider?.Settings == null)
                    return UpdateInterval.Weekly;

                if (GpoSettings?.UpdateInterval == null)
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

        private void VisitWebsiteExecute(object o)
        {
            try
            {
                _processStarter.Start(PdfforgeWebsiteUrl);
            }
            catch
            {
                // ignored
            }
        }
    }
}